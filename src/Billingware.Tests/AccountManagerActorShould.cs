using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using Akka.TestKit.Xunit2;
using Billingware.Common.Actors.Messages;
using Billingware.Common.Di;
using Billingware.Modules.Core.Actors;
using Newtonsoft.Json;
using Xunit;

namespace Billingware.Tests
{
    public class AccountManagerActorShould : TestKit
    {

        private IActorRef _accountManagerActorRef;

        public AccountManagerActorShould()
        {
            ApiDependencyResolverSystem.Start();
            var resolver = new SimpleInjectorDependencyResolver(ApiDependencyResolverSystem.GetContainer(), Sys);
            _accountManagerActorRef = Sys.ActorOf(Sys.DI().Props<AccountManagerActor>(), nameof(AccountManagerActor));

        }

        [Fact(DisplayName = "Create account")]
        public void CreateCustomerAccount()
        {

            // Arrange
            var json = new
            {
                allowOverdraft=true,
                active=true,
                branch="Kaneshie",
                accountType="SAVINGS"
            };

            var message = new CreateAccount("9237452718263673", "SAV_ACC", JsonConvert.SerializeObject(json));
            // Act
            _accountManagerActorRef.Tell(message);
            var res = ExpectMsg<AccountCreated>(TimeSpan.FromMinutes(2));
            // Assert
            Assert.Equal("200", res.Status.Code);

        }
    }
}