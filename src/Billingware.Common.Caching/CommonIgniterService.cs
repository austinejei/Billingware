using System;
using System.Collections.Generic;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Billingware.Models.Repository;
using Serilog;
using Serilog.Core;

namespace Billingware.Common.Caching
{
    public class CommonIgniterService
    {
        protected static readonly Logger Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();
        private static IIgnite _ignite;


       public static void Start(IgniteConfiguration conf, bool loadCacheFromDatabase = false)
        {

            _ignite = Ignition.Start(conf);

            StartCaches(loadCacheFromDatabase);
        }

        public static void StopAll() => Ignition.StopAll(false);
        public static void Stop(string name) => Ignition.Stop(name, false);

        public static void ReloadCaches()
        {
            
        }
       // public static IIgnite Instance => _ignite;
        private static void StartCaches(bool loadCacheFromDatabase)
        {
           StartCustomCaches();

            StartTransactionStatsCache(TransactionsStatsCacheRepository.CacheName,
                loadCacheFromDatabase);
        }

       

        public static void RefreshCache(string cacheName,
            bool loadCacheFromDatabase = false)
        {
            if (cacheName.Equals("*"))
            {
                StartCaches(loadCacheFromDatabase);
            }
         }

        private static void StartCustomCaches()
        {
          
        }

       

        private static void StartTransactionStatsCache(string name,
            bool loadCacheFromDatabase)
        {
            var cacheCfg = new CacheConfiguration
            {
                KeepBinaryInStore = false,  // Depends on your case
                CacheStoreFactory = new TransactionsStatsCacheStoreFactory(),
                Name = name,
            };

            TransactionsStatsCache = _ignite.GetOrCreateCache<string, object>(cacheCfg);
            if (loadCacheFromDatabase)
            {
                TransactionsStatsCache.Clear();
                TransactionsStatsCache.LoadCache(null);
            }
        }

     
        public static ICache<string, object> TransactionsStatsCache;
        
      
        public static void Dispose()
        {
            _ignite?.Dispose();
        }
    }
}
