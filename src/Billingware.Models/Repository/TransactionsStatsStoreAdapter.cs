using System;
using System.Linq;
using Apache.Ignite.Core.Cache.Store;

namespace Billingware.Models.Repository
{
    public class TransactionsStatsStoreAdapter : CacheStoreAdapter<string, object>
    {

        //private readonly ITransactionRepository _transactionRepository;

        public TransactionsStatsStoreAdapter()
        {
           // _transactionRepository = new TransactionRepository();
        }

        public override void LoadCache(Action<string, object> act,
            params object[] args)
        {
            var db = new BillingwareDataContext();

            var statsBase = db.Transactions.GroupBy(a => a.AccountNumber);

            //sum and count of all debits and credits
            act($"sum.debit", db.Transactions.Where(t => t.TransactionType == TransactionType.Debit).Select(s => s.Amount).DefaultIfEmpty(0).Sum());
            act($"sum.credit", db.Transactions.Where(t => t.TransactionType == TransactionType.Credit).Select(s => s.Amount).DefaultIfEmpty(0).Sum());
            act($"count", db.Transactions.LongCount());
            act($"count.credit", db.Transactions.Where(t=>t.TransactionType== TransactionType.Credit).LongCount());
            act($"count.debit", db.Transactions.Where(t=>t.TransactionType== TransactionType.Debit).LongCount());


            //sum and count of individual debit and credits
            foreach (var item in statsBase)
            {
                act($"count.{item.Key}", item.LongCount());
                act($"count.debit.{item.Key}", item.LongCount(t=>t.TransactionType== TransactionType.Debit));
                act($"count.credit.{item.Key}", item.LongCount(t=>t.TransactionType== TransactionType.Credit));
                
                act($"sum.debit.{item.Key}",item.Where(t=>t.TransactionType==TransactionType.Debit).Select(s=>s.Amount).DefaultIfEmpty(0).Sum());
                act($"sum.credit.{item.Key}",item.Where(t=>t.TransactionType==TransactionType.Credit).Select(s=>s.Amount).DefaultIfEmpty(0).Sum());
            }

        }

        public override object Load(string key)
        {
            return 0;
        }

        public override void Write(string key,
            object val)
        {

        }

        public override void Delete(string key)
        {

        }


    }
}