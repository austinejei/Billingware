using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Communication.Tcp;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Multicast;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Apache.Ignite.EntityFramework;
using Billingware.Common.Caching;

namespace Billingware.Models.Caching
{
    public class Efl2CustomCacheConfiguration : IgniteDbConfiguration
    {
        public Efl2CustomCacheConfiguration()
            : base(
                // IIgnite instance to use
                Ignition.Start(new IgniteConfiguration
                {
                    IgniteInstanceName = $"{typeof(BillingwareDataContext).FullName}.{typeof(BillingwareDataContext).Name}." + Guid.NewGuid().ToString("N"),
                    Logger = new CustomIgniteSerilogLogger(),
                    ClientMode = false,
                    ConsistentId = Guid.NewGuid().ToString("N"),
                    WorkDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    MetricsLogFrequency = TimeSpan.Zero,
                    MetricsUpdateFrequency = TimeSpan.Zero,
                    Localhost = "0.0.0.0",
                    DiscoverySpi = new TcpDiscoverySpi
                    {
                        IpFinder = new TcpDiscoveryStaticIpFinder()
                        {
                            Endpoints = new[]
                            {
                                "localhost"
                            }
                        },
                        LocalPort = 68500,
                        LocalPortRange = 10,
                    },
                    CommunicationSpi = new TcpCommunicationSpi
                    {
                        LocalPort = 68100
                    }

                }),
                // Metadata cache configuration (small cache, does not tolerate data loss)
                // Should be replicated or partitioned with backups
                new CacheConfiguration("metaCache")
                {
                    CacheMode = CacheMode.Replicated,
                    AtomicityMode = CacheAtomicityMode.Transactional
                },
                // Data cache configuration (large cache, holds actual query results, 
                // tolerates data loss). Can have no backups.
                new CacheConfiguration("dataCache")
                {
                    CacheMode = CacheMode.Partitioned,
                    Backups = 0
                },
                // Custom caching policy.
                new CustomCachingPolicy())
        {
            // No-op.
        }
    }
}