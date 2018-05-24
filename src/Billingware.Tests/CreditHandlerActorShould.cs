using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class CreditHandlerActorShould : TestKit
    {

        private IActorRef _creditActorRef;

        public CreditHandlerActorShould()
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
            _creditActorRef = Sys.ActorOf(Sys.DI().Props<CreditRequestActor>(), nameof(CreditRequestActor));
            var accountingActorRef = Sys.ActorOf(Sys.DI().Props<AccountingActor>(), nameof(AccountingActor));

            Sys.EventStream.Subscribe(accountingActorRef, typeof(DebitAccount));
            Sys.EventStream.Subscribe(accountingActorRef, typeof(CreditAccount));
            Sys.EventStream.Subscribe(accountingActorRef, typeof(PersistTransaction));

        }

        [Fact(DisplayName = "Credit 120 to an account")]
        public void RequestToCreditAccount()
        {

            // Arrange
            var reference = Guid.NewGuid().ToString("N");
            var message = new RequestAccountCredit("9237452718263673", reference, "deposit", decimal.Parse("120"),
                "234532", new Dictionary<string, string>().ToImmutableDictionary());
            // Act
            _creditActorRef.Tell(message);
            var res = ExpectMsg<AccountCreditResponse>(TimeSpan.FromMinutes(5));
            // Assert
            Assert.Equal("200", res.StatusResponse.Code);
            //Assert.Equal(decimal.Parse("10"), res.BalanceAfter);

        }
    }
}