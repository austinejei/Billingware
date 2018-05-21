namespace Billingware.Modules.PublicApi.Models
{
    public class DebitAccountResponseModel
    {
        public string AccountNumber { get; set; }
        public string Reference { get; set; }
        public string Ticket { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
    }
}