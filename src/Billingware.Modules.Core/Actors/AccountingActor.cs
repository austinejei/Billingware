using System.Data.Entity;
using System.Linq;
using Billingware.Common.Actors;
using Billingware.Models;
using Billingware.Models.Core;
using Billingware.Models.Repository;
using Billingware.Modules.Core.Events;

namespace Billingware.Modules.Core.Actors
{
    public class AccountingActor:BaseActor
    {
        private readonly ITranactionsStatsCacheRepository _tranactionsStatsCache;

        public AccountingActor(ITranactionsStatsCacheRepository tranactionsStatsCache)
        {
            _tranactionsStatsCache = tranactionsStatsCache;
            Receive<DebitAccount>(x => DoDebitAccountEvent(x));
            Receive<PersistTransaction>(x => DoPersistTransaction(x));
        }

        private void DoPersistTransaction(PersistTransaction req)
        {
            using (var db = new BillingwareDataContext())
            {
                db.Transactions.Add(new Transaction
                {
                    AccountNumber = req.AccountNumber,
                    Amount = req.Amount,
                    TransactionType = req.TransactionType,
                    Reference = req.Reference,
                    BalanceAfter = req.BalanceAfter,
                    BalanceBefore = req.BalanceBefore,
                    ClientId = req.ClientId,
                    ConditionFailReason = req.ConditionFailReason,
                    CreatedAt = req.CreatedAt,
                    Honoured = req.Honoured,
                    Narration =req.Narration,
                    SatisfiedCondition = req.SatisfiedCondition,
                    Ticket = req.Ticket
                });
                db.SaveChanges();
            }

            //increase cache items
            var typeKey = req.TransactionType == TransactionType.Credit ? "credit" : "debit";
            _tranactionsStatsCache.Increase($"count", int.Parse("1"));
            _tranactionsStatsCache.Increase($"count.{typeKey}", int.Parse("1"));
            _tranactionsStatsCache.Increase($"count.{typeKey}.{req.AccountNumber}", int.Parse("1"));

            _tranactionsStatsCache.Increase($"sum.{typeKey}", req.Amount);
            _tranactionsStatsCache.Increase($"sum.{typeKey}.{req.AccountNumber}", req.Amount);
        }

        private void DoDebitAccountEvent(DebitAccount detail)
        {
            using (var db = new BillingwareDataContext())
            {
                var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == detail.Request.AccountNumber);

                if (account==null)
                {
                    return;
                }

                account.Balance -= detail.Request.Amount;

                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}