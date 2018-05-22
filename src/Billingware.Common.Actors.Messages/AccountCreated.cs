namespace Billingware.Common.Actors.Messages
{
    public struct AccountCreated
    {
        public CommonStatusResponse Status { get; }

        public AccountCreated(CommonStatusResponse status)
        {
            Status = status;
        }
    }
}