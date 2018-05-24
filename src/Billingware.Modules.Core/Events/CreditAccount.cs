using Billingware.Common.Actors.Messages;

namespace Billingware.Modules.Core.Events
{
    public struct CreditAccount
    {
        public RequestAccountCredit Request { get; }

        public CreditAccount(RequestAccountCredit request)
        {
            Request = request;
        }
    }
}