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
