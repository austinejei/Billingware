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
using Billingware.Common.Actors.Messages;
using Billingware.Common.Di;
using Billingware.Modules.Core.Actors;
using Xunit;

namespace Billingware.Tests
{
    public class DebitHandlerActorShould : TestKit
    {

        private IActorRef _debitActorRef;
        
        public DebitHandlerActorShould()
        {
            ApiDependencyResolverSystem.Start();
            var resolver = new SimpleInjectorDependencyResolver(ApiDependencyResolverSystem.GetContainer(), Sys);
            _debitActorRef = Sys.ActorOf(Sys.DI().Props<DebitRequestActor>(), nameof(DebitRequestActor));

        }

        [Fact(DisplayName = "Debit 10 from an account")]
        public void RequestToDebitAccount()
        {

            // Arrange
            var reference = Guid.NewGuid().ToString("N");
            var message = new RequestAccountDebit("234542", reference, "money transfer charge", decimal.Parse("10"),
                "234532", new Dictionary<string, string>().ToImmutableDictionary());
            // Act
            _debitActorRef.Tell(message);
             var res = ExpectMsg<AccountDebitResponse>(TimeSpan.FromMinutes(2));
            // Assert
            Assert.Equal("200", res.StatusResponse.Code);

        }
    }
}
