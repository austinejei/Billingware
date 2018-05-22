using Billingware.Common.Actors.Messages;

namespace Billingware.Modules.Core.Events
{
    public struct DebitAccount
    {
        public RequestAccountDebit Request { get; }

        public DebitAccount(RequestAccountDebit request)
        {
            Request = request;
        }
    }
}