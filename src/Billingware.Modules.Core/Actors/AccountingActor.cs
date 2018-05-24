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
            Receive<CreditAccount>(x => DoCreditAccountEvent(x));
            Receive<PersistTransaction>(x => DoPersistTransaction(x));
        }

        private void DoPersistTransaction(PersistTransaction req)
        {
            using (var db = new BillingwareDataContext())
            {
                var t = new Transaction
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
                    Narration = req.Narration,
                    SatisfiedCondition = req.SatisfiedCondition,
                    Ticket = req.Ticket
                };

                //quick fix 
                if (t.TransactionType== TransactionType.Debit)
                {
                    //if balance = 100, amount=10 =>  after=90 and before=100
                    var sumBefore = t.BalanceAfter + t.Amount;

                    if (t.BalanceBefore != sumBefore)
                    {
                        t.BalanceBefore = t.BalanceAfter + (t.Amount);
                    }
                }
                else
                {
                    //if balance = 90, amount=10 =>  after=100 and before=90
                    var sumBefore = t.BalanceAfter - t.Amount;

                    if (t.BalanceBefore != sumBefore)
                    {
                        t.BalanceBefore = t.BalanceAfter - (t.Amount);
                    }
                }
               
                db.Transactions.Add(t);
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

        private void DoCreditAccountEvent(CreditAccount detail)
        {
            using (var db = new BillingwareDataContext())
            {
                var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == detail.Request.AccountNumber);

                if (account==null)
                {
                    return;
                }

                account.Balance += detail.Request.Amount;

                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}