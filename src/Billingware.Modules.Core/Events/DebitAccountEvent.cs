using Billingware.Common.Actors.Messages;

namespace Billingware.Modules.Core.Events
{
    public struct DebitAccountEvent
    {
        public RequestAccountDebit Request { get; }

        public DebitAccountEvent(RequestAccountDebit request)
        {
            Request = request;
        }
    }
}