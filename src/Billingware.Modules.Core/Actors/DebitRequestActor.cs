using System;
using System.Collections.Generic;
using System.Linq;
using Billingware.Common.Actors;
using Billingware.Common.Actors.Messages;
using Billingware.Models;
using Billingware.Models.Core;
using Billingware.Models.Repository;
using Billingware.Modules.Core.Events;
using Billingware.Modules.Core.Helpers;

namespace Billingware.Modules.Core.Actors
{
    public class DebitRequestActor:BaseActor
    {
        private readonly ITranactionsStatsCacheRepository _tranactionsStatsCache;

        public DebitRequestActor(ITranactionsStatsCacheRepository tranactionsStatsCache)
        {
            _tranactionsStatsCache = tranactionsStatsCache;
            Receive<RequestAccountDebit>(x => DoRequestAccountDebit(x));
        }

        private void DoRequestAccountDebit(RequestAccountDebit request)
        {
            Log(CommonLogLevel.Debug, $"received request to debit {request.AccountNumber} with {request.Amount}", null,
                null);


            //we generate a unique ticket for every transaction
            var ticket = Guid.NewGuid().ToString("N");

            /*
             * 1. Find the account
             * 2. Execute conditions on account - we shall use Expression Trees
             * 3. Apply outcome
             * 4. Persist results
             */
            var db = new BillingwareDataContext();

            var account = db.Accounts.AsNoTracking().FirstOrDefault(a => a.AccountNumber == request.AccountNumber);

            if (account==null)
            {
                Sender.Tell(
                    new AccountDebitResponse(request.Reference, request.AccountNumber, ticket, request.Amount, decimal.Zero,
                        decimal.Zero, new CommonStatusResponse(message: $"account {request.AccountNumber} not found.",code:"404",subCode:"404.1")), Self);
                return;
            }

            var generalConditions =
                    db.Conditions.AsNoTracking().Where(c => c.Active && c.ConditionApplicatorType == ConditionApplicatorType.All).ToList()
                ;

            var specificConditions = db.Conditions.AsNoTracking().Where(c =>
                c.Active && c.ConditionApplicatorType == ConditionApplicatorType.One &&
                c.AppliedToAccountNumbers.Contains(request.AccountNumber)).ToList();

            var allConditions = new List<Condition>();

            if (generalConditions.Any())
            {
                allConditions.AddRange(generalConditions);
            }

            if (specificConditions.Any())
            {
                allConditions.AddRange(specificConditions);
            }

            
            /***
             * how?
             * 1. check cache, if not, then create entry
             * 2. if in cache, just load it up
             */
            var allTransactionsCount = (long)_tranactionsStatsCache.Find($"count.{request.AccountNumber}").Result;
            var debitTransactionSum = (long) _tranactionsStatsCache.Find($"sum.debit.{request.AccountNumber}").Result;
            var creditTransactionSum = (long)_tranactionsStatsCache.Find($"sum.credit.{request.AccountNumber}").Result;
            var debitTransactionCount = (long)_tranactionsStatsCache.Find($"count.debit.{request.AccountNumber}").Result;
            var creditTransactionCount = (long)_tranactionsStatsCache.Find($"count.credit.{request.AccountNumber}").Result;


            var outcomeList = ConditionEvaluatorHelper.EvaluateConditionAndGetOutcomeIds(account, allConditions,new ClientPayloadData("debit",request.Reference,request.Amount),
                creditTransactionSum, creditTransactionCount, debitTransactionSum, debitTransactionCount,
                allTransactionsCount);



            if (outcomeList.Any())
            {
                //todo: check if there's any "halt" type in the outcome list...then,
                //raise an event to actually inspect the outcome of the condition evaluation
                //it would also increment the stats cache values
                var outcomes = db.Outcomes.AsNoTracking().Where(o => outcomeList.Contains(o.Id) && o.Active)
                    .OrderBy(i => i.Id);

                if (!outcomes.Any())
                {
                    Sender.Tell(
                        new AccountDebitResponse(request.Reference, request.AccountNumber, ticket, request.Amount, decimal.Zero,
                            decimal.Zero, new CommonStatusResponse(message: $"condition was satisfied but no active outcome could be applied.", code: "500", subCode: "500.1")), Self);
                    return;
                }


                if (outcomes.Any(o=>o.OutcomeType==OutcomeType.Halt))
                {
                    Sender.Tell(
                        new AccountDebitResponse(request.Reference, request.AccountNumber, ticket, request.Amount, decimal.Zero,
                            decimal.Zero, new CommonStatusResponse(message: $"condition was satisfied. but outcome HALTs the process", code: "403", subCode: "403.1")), Self);
                    return;
                }
                
                Publish(new DebitAccountEvent(request));
                Sender.Tell(
                    new AccountDebitResponse(request.Reference, request.AccountNumber, ticket, request.Amount, decimal.Zero,
                        decimal.Zero, new CommonStatusResponse(message: "Successful")), Self);
                return;
            }

            Sender.Tell(
                new AccountDebitResponse(request.Reference, request.AccountNumber, ticket, request.Amount, decimal.Zero,
                    decimal.Zero, new CommonStatusResponse(message: $"condition was not satisfied.", code: "400", subCode: "404.1")), Self);
            return;
        }
    }
}