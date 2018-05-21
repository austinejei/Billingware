using Billingware.Common.Actors;
using Billingware.Common.Actors.Messages;

namespace Billingware.Modules.Core.Actors
{
    public class DebitHandlerActor:BaseActor
    {
        public DebitHandlerActor()
        {
            Receive<RequestAccountDebit>(x => DoRequestAccountDebit(x));
        }

        private void DoRequestAccountDebit(RequestAccountDebit request)
        {
            Log(CommonLogLevel.Debug, $"received request to debit {request.AccountNumber} with {request.Amount}", null,
                null);
        }
    }
}