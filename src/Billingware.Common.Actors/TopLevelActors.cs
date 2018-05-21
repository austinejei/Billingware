using Akka.Actor;

namespace Billingware.Common.Actors
{
    public class TopLevelActors
    {
        public static IActorRef DebitHandlerActor = ActorRefs.Nobody;
        public static IActorRef CreditsHandlerActor = ActorRefs.Nobody;
        public static IActorRef AccountsHandlerActor = ActorRefs.Nobody;

    }
}