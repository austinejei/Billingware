using System.ComponentModel.DataAnnotations;

namespace Billingware.Modules.PublicApi.Models
{
    /// <summary>
    /// The model used to request for account debit
    /// </summary>
    public class DebitAccountRequestModel
    {
        /// <summary>
        /// Must be valid and not null
        /// </summary>
        [Required]
        public string Reference { get; set; }
        /// <summary>
        /// Recommended but not required.
        /// </summary>
        public string Narration { get; set; }
        
        /// <summary>
        /// Ideally a non-negative value.
        /// </summary>
        public decimal Amount { get; set; }
    }
}