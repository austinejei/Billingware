namespace Billingware.Common.Actors.Messages
{
    public class CommonStatusResponse
    {
        public string Code { get; }
        public string SubCode { get; }
        public string Message { get; }

        public CommonStatusResponse(string code="200",string subCode="200.1",string message="")
        {
            Code = code;
            SubCode = subCode;
            Message = message;
        }
    }
}