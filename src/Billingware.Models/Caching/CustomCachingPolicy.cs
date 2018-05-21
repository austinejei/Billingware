using System;
using Apache.Ignite.EntityFramework;

namespace Billingware.Models.Caching
{
    public class CustomCachingPolicy : IDbCachingPolicy
    {
        /// <summary>
        /// Determines whether the specified query can be cached.
        /// </summary>
        public virtual bool CanBeCached(DbQueryInfo queryInfo)
        {
            // This method is called before database call.
            // Cache only Persons.
           // return queryInfo.AffectedEntitySets.All(x => x.Name == "Person");
            return true;
        }

        /// <summary>
        /// Determines whether specified number of rows should be cached.
        /// </summary>
        public virtual bool CanBeCached(DbQueryInfo queryInfo, int rowCount)
        {
            // This method is called after database call.
            // Cache only queries that return less than 1000 rows.
            return rowCount < 1000;
        }

        /// <summary>
        /// Gets the absolute expiration timeout for a given query.
        /// </summary>
        public virtual TimeSpan GetExpirationTimeout(DbQueryInfo queryInfo)
        {
            // Cache for 5 minutes.
            return TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// Gets the caching strategy for a given query.
        /// </summary>
        public virtual DbCachingMode GetCachingMode(DbQueryInfo queryInfo)
        {
            // Cache with invalidation.
            return DbCachingMode.ReadWrite;
        }
    }
}