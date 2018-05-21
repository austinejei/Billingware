using System.ComponentModel.DataAnnotations;

namespace Billingware.Modules.PublicApi.Models
{
    public class DebitAccountRequestModel
    {
        [Required]
        public string Reference { get; set; }
        public string Narration { get; set; }
        
        public decimal Amount { get; set; }
    }
}