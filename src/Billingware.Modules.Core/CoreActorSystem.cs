using System;
using System.Collections.Generic;
using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Akka.DI.Core;
using Akka.DI.SimpleInjector;
using Akka.Routing;
using Billingware.Common.Actors;
using Billingware.Common.Di;
using Billingware.Modules.Core.Actors;
using Billingware.Modules.Core.Events;

namespace Billingware.Modules.Core
{
    public class CoreActorSystem
    {
        public static List<IActorRef> NotificationChannelsActors;
        public static List<IActorRef> TemplateParsersActors;
        private static ActorSystem _actorSystem;
        private static SimpleInjectorDependencyResolver _resolver;

        private static SupervisorStrategy GetDefaultSupervisorStrategy
        {
            get
            {
                return new OneForOneStrategy(3,
                    TimeSpan.FromSeconds(3),
                    ex =>
                    {
                        //CommonLogger.Error<AppsRemoteActorSystem>(ex,ex.Message);
                        if (ex is ActorInitializationException)
                        {
                            Stop();
                            return Directive.Stop;
                        }

                        return Directive.Resume;
                    });
            }
        }

        /// <summary>
        /// This method starts the Core ActorSystem
        /// </summary>
        public static void Start()
        {
            var conf = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\core-remote-server.conf");
            _actorSystem = ActorSystem.Create(nameof(CoreActorSystem), ConfigurationFactory.ParseString(conf));

            ApiDependencyResolverSystem.Start();

            _resolver = new SimpleInjectorDependencyResolver(ApiDependencyResolverSystem.GetContainer(), _actorSystem);



            TopLevelActors.DebitHandlerActor = _actorSystem.ActorOf(_actorSystem.DI()
                    .Props<DebitRequestActor>()
                    .WithSupervisorStrategy(GetDefaultSupervisorStrategy)
                    .WithRouter(FromConfig.Instance),
                nameof(DebitRequestActor));

            TopLevelActors.CreditsHandlerActor = _actorSystem.ActorOf(_actorSystem.DI()
                    .Props<CreditRequestActor>()
                    .WithSupervisorStrategy(GetDefaultSupervisorStrategy)
                    .WithRouter(FromConfig.Instance),
                nameof(CreditRequestActor));

            TopLevelActors.AccountingActor = _actorSystem.ActorOf(_actorSystem.DI()
                    .Props<AccountingActor>()
                    .WithSupervisorStrategy(GetDefaultSupervisorStrategy)
                    .WithRouter(FromConfig.Instance),
                nameof(AccountingActor));



            _actorSystem.EventStream.Subscribe(TopLevelActors.AccountingActor, typeof(DebitAccount));
            _actorSystem.EventStream.Subscribe(TopLevelActors.AccountingActor, typeof(CreditAccount));
            _actorSystem.EventStream.Subscribe(TopLevelActors.AccountingActor, typeof(PersistTransaction));
        }

        /// <summary>
        /// This method stops the actor system
        /// </summary>
        public static void Stop()
        {
            _actorSystem.Terminate().Wait(1000);
        }


    }
}