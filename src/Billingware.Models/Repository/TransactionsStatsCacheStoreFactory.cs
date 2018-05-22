using Apache.Ignite.Core.Common;

namespace Billingware.Models.Repository
{
    public class TransactionsStatsCacheStoreFactory : IFactory<TransactionsStatsStoreAdapter>
    {
        public TransactionsStatsStoreAdapter CreateInstance()
        {

            return new TransactionsStatsStoreAdapter();
        }
    }
}