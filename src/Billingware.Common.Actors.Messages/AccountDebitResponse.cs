namespace Billingware.Common.Actors.Messages
{
    public struct AccountDebitResponse
    {
        public string Reference { get; }
        public string AccountNumber { get; }
        public string Ticket { get; }
        public decimal Amount { get; }
        public decimal BalanceBefore { get; }
        public decimal BalanceAfter { get; }
        public CommonStatusResponse StatusResponse { get; }

        public AccountDebitResponse(string reference,string accountNumber,string ticket,decimal amount,decimal balanceBefore,decimal balanceAfter,CommonStatusResponse statusResponse)
        {
            Reference = reference;
            AccountNumber = accountNumber;
            Ticket = ticket;
            Amount = amount;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
            StatusResponse = statusResponse;
        }
    }

    public struct AccountCreditResponse
    {
        public string Reference { get; }
        public string AccountNumber { get; }
        public string Ticket { get; }
        public decimal Amount { get; }
        public decimal BalanceBefore { get; }
        public decimal BalanceAfter { get; }
        public CommonStatusResponse StatusResponse { get; }

        public AccountCreditResponse(string reference, string accountNumber, string ticket, decimal amount, decimal balanceBefore, decimal balanceAfter, CommonStatusResponse statusResponse)
        {
            Reference = reference;
            AccountNumber = accountNumber;
            Ticket = ticket;
            Amount = amount;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
            StatusResponse = statusResponse;
        }
    }
}