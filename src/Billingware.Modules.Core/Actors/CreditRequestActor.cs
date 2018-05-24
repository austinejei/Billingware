using System;
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
    public class CreditRequestActor : BaseActor
    {
        private readonly ITranactionsStatsCacheRepository _tranactionsStatsCache;

        public CreditRequestActor(ITranactionsStatsCacheRepository tranactionsStatsCache)
        {
            _tranactionsStatsCache = tranactionsStatsCache;
            Receive<RequestAccountCredit>(x => DoRequestAccountCredit(x));
        }

        private void DoRequestAccountCredit(RequestAccountCredit request)
        {
            Log(CommonLogLevel.Debug, $"received request to credit {request.AccountNumber} with {request.Amount}", null,
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

            if (account == null)
            {
                Sender.Tell(
                    new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, decimal.Zero,
                        decimal.Zero, new CommonStatusResponse(message: $"account {request.AccountNumber} not found.", code: "404", subCode: "404.1")), Self);
                return;
            }

            var generalConditions =
                    db.Conditions.AsNoTracking().Where(c => c.Active && c.ConditionApplicatorType == ConditionApplicatorType.All).ToList()
                ;

            var specificConditions = db.Conditions.AsNoTracking().Where(c =>
                c.Active && c.ConditionApplicatorType == ConditionApplicatorType.One &&
                c.AppliedToAccountNumbers.Contains(request.AccountNumber)).ToList();





            var allTransactionsCount = long.Parse(_tranactionsStatsCache.Find($"count.{request.AccountNumber}").Result.ToString());
            var debitTransactionSum = decimal.Parse(_tranactionsStatsCache.Find($"sum.debit.{request.AccountNumber}").Result.ToString());
            var creditTransactionSum = decimal.Parse(_tranactionsStatsCache.Find($"sum.credit.{request.AccountNumber}").Result.ToString());
            var debitTransactionCount = long.Parse(_tranactionsStatsCache.Find($"count.debit.{request.AccountNumber}").Result.ToString());
            var creditTransactionCount = long.Parse(_tranactionsStatsCache.Find($"count.credit.{request.AccountNumber}").Result.ToString());

            // var allConditions = new List<Condition>();

            bool passedSpecificConditions = false;
            if (specificConditions.Any())
            {
                //allConditions.AddRange(specificConditions);

                var perAccountOutcomeList = ConditionEvaluatorHelper.EvaluateConditionAndGetOutcomeIds(account, specificConditions, new ClientPayloadData("credit", request.Reference, request.Amount),
                    creditTransactionSum, creditTransactionCount, debitTransactionSum, debitTransactionCount,
                    allTransactionsCount);


                if (perAccountOutcomeList.Any())
                {
                    //check if there's any "halt" type in the outcome list...then,
                    //raise an event to actually inspect the outcome of the condition evaluation
                    //it would also increment the stats cache values
                    var outcomes = db.Outcomes.AsNoTracking().Where(o => perAccountOutcomeList.Contains(o.Id) && o.Active)
                        .OrderBy(i => i.Id);

                    if (!outcomes.Any())
                    {
                        Sender.Tell(
                            new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance, account.Balance
                                , new CommonStatusResponse(message: $"condition was satisfied but no active outcome could be applied.", code: "500", subCode: "500.1")), Self);
                        return;
                    }


                    if (outcomes.Any(o => o.OutcomeType == OutcomeType.Halt))
                    {
                        Sender.Tell(
                            new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                                account.Balance, new CommonStatusResponse(message: $"condition was satisfied. but at least one outcome HALTs the process", code: "403", subCode: "403.1")), Self);
                        return;
                    }

                    //we need to also run the general conditions...
                    passedSpecificConditions = true;
                    //Publish(new CreditAccount(request));

                    //Publish(new PersistTransaction(request.AccountNumber, DateTime.Now, request.Narration, request.Amount,
                    //    account.Balance, account.Balance + request.Amount, TransactionType.Credit, true, string.Empty, true,
                    //    request.ClientId, request.Reference, ticket));

                    //Sender.Tell(
                    //    new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                    //        account.Balance + request.Amount, new CommonStatusResponse(message: "Successful")), Self);
                    //return;
                }
            }

            if (generalConditions.Any())
            {
                // allConditions.AddRange(generalConditions);

                var generalOutcomeList = ConditionEvaluatorHelper.EvaluateConditionAndGetOutcomeIds(account, generalConditions, new ClientPayloadData("credit", request.Reference, request.Amount),
                    creditTransactionSum, creditTransactionCount, debitTransactionSum, debitTransactionCount,
                    allTransactionsCount);


                if (generalOutcomeList.Any())
                {
                    //check if there's any "halt" type in the outcome list...then,
                    //raise an event to actually inspect the outcome of the condition evaluation
                    //it would also increment the stats cache values
                    var outcomes = db.Outcomes.AsNoTracking().Where(o => generalOutcomeList.Contains(o.Id) && o.Active)
                        .OrderBy(i => i.Id);


                    
                    if (!outcomes.Any())
                    {
                        if (passedSpecificConditions)
                        {
                            Publish(new CreditAccount(request));

                            Publish(new PersistTransaction(request.AccountNumber, DateTime.Now, request.Narration, request.Amount,
                                account.Balance, account.Balance + request.Amount, TransactionType.Credit, true, string.Empty, true,
                                request.ClientId, request.Reference, ticket));

                            Sender.Tell(
                                new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                                    account.Balance + request.Amount, new CommonStatusResponse(message: "Successful")), Self);
                            return;
                        }

                        Sender.Tell(
                            new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                                account.Balance, new CommonStatusResponse(message: $"condition was satisfied but no active outcome could be applied.", code: "500", subCode: "500.1")), Self);
                        return;
                    }


                    if (outcomes.Any(o => o.OutcomeType == OutcomeType.Halt))
                    {
                        //but....
                        if (passedSpecificConditions)
                        {
                            Publish(new CreditAccount(request));

                            Publish(new PersistTransaction(request.AccountNumber, DateTime.Now, request.Narration, request.Amount,
                                account.Balance, account.Balance + request.Amount, TransactionType.Credit, true, string.Empty, true,
                                request.ClientId, request.Reference, ticket));

                            Sender.Tell(
                                new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                                    account.Balance + request.Amount, new CommonStatusResponse(message: "Successful")), Self);
                            return;
                        }
                        Sender.Tell(
                            new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                                account.Balance, new CommonStatusResponse(message: $"condition was satisfied. but at least one outcome HALTs the process", code: "403", subCode: "403.1")), Self);
                        return;
                    }

                    Publish(new CreditAccount(request));

                    Publish(new PersistTransaction(request.AccountNumber, DateTime.Now, request.Narration, request.Amount,
                        account.Balance, account.Balance + request.Amount, TransactionType.Credit, true, string.Empty, true,
                        request.ClientId, request.Reference, ticket));

                    Sender.Tell(
                        new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                            account.Balance + request.Amount, new CommonStatusResponse(message: "Successful")), Self);
                    return;
                }
            }


            //if there are no conditions so we still proceed with the intended action
            Publish(new CreditAccount(request));

            Publish(new PersistTransaction(request.AccountNumber, DateTime.Now, request.Narration, request.Amount,
                account.Balance, account.Balance + request.Amount, TransactionType.Credit, false, "no condition", true,
                request.ClientId, request.Reference, ticket));

            Sender.Tell(
                new AccountCreditResponse(request.Reference, request.AccountNumber, ticket, request.Amount, account.Balance,
                    account.Balance - request.Amount, new CommonStatusResponse(message: "Successful")), Self);
            return;
        }
    }
}