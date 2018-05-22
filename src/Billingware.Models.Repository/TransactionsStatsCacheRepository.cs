using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billingware.Models.Repository
{
    public class TransactionsStatsCacheRepository : CacheBase, ITranactionsStatsCacheRepository
    {


        public static string CacheName => GetTableName(typeof(TransactionStat));
        public ICache<string, object> Cache { get; }

        public TransactionsStatsCacheRepository(ICache<string, object> cache) { Cache = cache; }
        public Task<Dictionary<string, object>> All
        {
            get
            {
                return Task.FromResult(Cache.ToDictionary(k => k.Key,
                    v => v.Value));
            }
        }

        public Task<object> Find(string key)
        {
            if (!Cache.ContainsKey(key))
            {
                if (key.Contains("sum") || key.Contains("count"))
                {
                    return Task.FromResult<object>(0);
                }
                return Task.FromResult<object>(new { });
            }
            return Cache.GetAsync(key);
        }

        public Task Increase(string key,
            object value)
        {
            if (!Cache.ContainsKey(key))
            {
                return Cache.PutAsync(key,
                    value);

            }
            var item = Cache.Get(key);

            if (value is int i)
            {
                var v = (int)item;
                v += i;

                return Cache.PutAsync(key,
                    v);
            }

            if (value is decimal @decimal)
            {
                var v = (decimal)item;
                v += @decimal;

                return Cache.PutAsync(key,
                    v);
            }

            return Task.FromResult(0);
        }

        public Task Decrease(string key,
            object value)
        {
            if (!Cache.ContainsKey(key))
            {
                return Cache.PutAsync(key,
                    value);
            }
            var item = Cache.Get(key);

            if (value is int i)
            {
                var v = (int)item;
                v -= i;

                return Cache.PutAsync(key,
                    v);
            }

            if (value is decimal @decimal)
            {
                var v = (decimal)item;
                v -= @decimal;

                return Cache.PutAsync(key,
                    v);
            }

            return Task.FromResult(0);
        }

        public void Reload()
        {
            Cache.Clear();
            var all = All.Result;

            foreach (var item in all)
            {
                if (item.Key.Contains("list"))
                {
                    continue;
                }

                if (item.Value is int)
                {

                    Cache.Put(item.Key,
                        (int)0);
                }

                if (item.Value is decimal)
                {
                    Cache.Put(item.Key,
                        decimal.Zero);
                }
            }

            Cache.LoadCache(null);
        }

    }
}