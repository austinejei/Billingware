using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Billingware.Models.Core
{
    public class Account
    {
        public int Id { get; set; }

        /// <summary>
        /// Must be unique and not null
        /// </summary>
        [Required,StringLength(50),Index]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Another caption for the account
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// JSON field
        /// </summary>
        public string Extra { get; set; }

        /// <summary>
        /// The actual amount. Can be 0. 
        /// </summary>
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}