namespace Billingware.Modules.PublicApi.Models
{
    /// <summary>
    /// Debit response model after a request has been inspected and validated
    /// </summary>
    public class DebitAccountResponseModel
    {
        /// <summary>
        /// The account number
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// The caller's reference
        /// </summary>
        public string Reference { get; set; }
        /// <summary>
        /// System-generated ticket. Can be used to check the finalized status of the debit request
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// The intended debit amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// The balance before the transaction was honoured just after validation
        /// </summary>
        public decimal BalanceBefore { get; set; }
        /// <summary>
        /// The balance after the transaction was honoured
        /// </summary>
        public decimal BalanceAfter { get; set; }
    }
}