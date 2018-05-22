using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billingware.Common.Actors.Messages
{
    public struct RequestAccountDebit
    {
        public string AccountNumber { get; }
        public string Reference { get; }
        public string Narration { get; }
        public decimal Amount { get; }
        public string ClientId { get; }
        /// <summary>
        /// may contain other identifiable info e.g. IP address
        /// </summary>
        public ImmutableDictionary<string, string> Extras { get; }
        public RequestAccountDebit(string accountNumber,string reference,string narration,decimal amount,string clientId,ImmutableDictionary<string,string> extras)
        {
            AccountNumber = accountNumber;
            Reference = reference;
            Narration = narration;
            Amount = amount;
            ClientId = clientId;
            Extras = extras;
        }
    }
}
