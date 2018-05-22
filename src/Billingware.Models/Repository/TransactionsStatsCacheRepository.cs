using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apache.Ignite.Core.Cache;

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

            if (key.Contains("count"))
            {
                var v = long.Parse(item.ToString());
                v += long.Parse(value.ToString());

                return Cache.PutAsync(key,
                    v);
            }
          
            if (key.Contains("sum"))
            {
                var v = decimal.Parse(item.ToString());
                v += decimal.Parse(value.ToString());

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

            if (key.Contains("count"))
            {
                var v = long.Parse(item.ToString());
                v -= long.Parse(value.ToString());

                return Cache.PutAsync(key,
                    v);
            }

            if (key.Contains("sum"))
            {
                var v = decimal.Parse(item.ToString());
                v -= decimal.Parse(value.ToString());

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

                if (item.Key.Contains("count"))
                {

                    Cache.Put(item.Key,
                        long.Parse("0"));
                }

                if (item.Key.Contains("sum"))
                {
                    Cache.Put(item.Key,
                        decimal.Zero);
                }
            }

            Cache.LoadCache(null);
        }

    }
}