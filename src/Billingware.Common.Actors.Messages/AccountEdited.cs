namespace Billingware.Common.Actors.Messages
{
    public struct AccountEdited
    {
        public CommonStatusResponse Status { get; }

        public AccountEdited(CommonStatusResponse status)
        {
            Status = status;
        }
    }
}