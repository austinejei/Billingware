using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billingware.Models.Repository
{
    public interface ITranactionsStatsCacheRepository
    {
        Task<Dictionary<string, object>> All { get; }

        Task<object> Find(string key);

        Task Increase(string key,
            object value);

        Task Decrease(string key,
            object value);

        void Reload();
    }
}