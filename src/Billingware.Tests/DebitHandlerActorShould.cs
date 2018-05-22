using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using Akka.TestKit.Xunit2;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Communication.Tcp;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Static;
using Billingware.Common.Actors.Messages;
using Billingware.Common.Caching;
using Billingware.Common.Di;
using Billingware.Common.Logging.Ignite;
using Billingware.Modules.Core.Actors;
using Billingware.Modules.Core.Events;
using Xunit;

namespace Billingware.Tests
{
    public class DebitHandlerActorShould : TestKit
    {

        private IActorRef _debitActorRef;
        
        public DebitHandlerActorShould()
        {
            var conf = new IgniteConfiguration
            {
                IgniteInstanceName = "Billingware.Cache.Tests." + Guid.NewGuid().ToString("N"),
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
            ApiDependencyResolverSystem.Start();
            var resolver = new SimpleInjectorDependencyResolver(ApiDependencyResolverSystem.GetContainer(), Sys);
            _debitActorRef = Sys.ActorOf(Sys.DI().Props<DebitRequestActor>(), nameof(DebitRequestActor));
            var accountingActorRef = Sys.ActorOf(Sys.DI().Props<AccountingActor>(), nameof(AccountingActor));

            Sys.EventStream.Subscribe(accountingActorRef, typeof(DebitAccount));
            Sys.EventStream.Subscribe(accountingActorRef, typeof(PersistTransaction));

        }

        [Fact(DisplayName = "Debit 10 from an account")]
        public void RequestToDebitAccount()
        {

            // Arrange
            var reference = Guid.NewGuid().ToString("N");
            var message = new RequestAccountDebit("9237452718263673", reference, "withdrawal", decimal.Parse("10"),
                "234532", new Dictionary<string, string>().ToImmutableDictionary());
            // Act
            _debitActorRef.Tell(message);
             var res = ExpectMsg<AccountDebitResponse>(TimeSpan.FromMinutes(5));
            // Assert
            Assert.Equal("200", res.StatusResponse.Code);

        }
    }
}
