using System;
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