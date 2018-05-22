using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Communication.Tcp;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Billingware.Common.Caching;
using Billingware.Common.Logging.Ignite;
using Billingware.Modules.Common;

namespace Billingware.Modules.Caching
{
    public class SimpleCacheModule: IBillingwareModule
    {
        public void Start()
        {
            
           
            var conf = new IgniteConfiguration
            {
                IgniteInstanceName = "Billingware.Cache.Main." + Guid.NewGuid().ToString("N"),
                Logger = new CustomIgniteSerilogLogger(),
                ClientMode = false,
               // ConsistentId = Guid.NewGuid().ToString("N"),
                WorkDirectory = AppDomain.CurrentDomain.BaseDirectory,
                MetricsLogFrequency = TimeSpan.Zero,
               // MetricsUpdateFrequency = TimeSpan.Zero,
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
                    LocalPort = 48500,
                    LocalPortRange = 10,
                },
                CommunicationSpi = new TcpCommunicationSpi
                {
                    LocalPort = 48100
                }
            };
            CommonIgniterService.Start(conf, true);
        }

        public void Stop()
        {
            CommonIgniterService.Dispose();
        }
    }
}
