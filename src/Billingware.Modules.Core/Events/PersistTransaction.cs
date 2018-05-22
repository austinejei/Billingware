using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Billingware.Models;

namespace Billingware.Modules.Core.Events
{
    public struct PersistTransaction
    {
        public PersistTransaction(string accountNumber, DateTime createdAt, string narration, decimal amount, decimal balanceBefore, decimal balanceAfter, TransactionType transactionType, bool satisfiedCondition, string conditionFailReason, bool honoured, string clientId, string reference, string ticket)
        {
            AccountNumber = accountNumber;
            CreatedAt = createdAt;
            Narration = narration;
            Amount = amount;
            BalanceBefore = balanceBefore;
            BalanceAfter = balanceAfter;
            TransactionType = transactionType;
            SatisfiedCondition = satisfiedCondition;
            ConditionFailReason = conditionFailReason;
            Honoured = honoured;
            ClientId = clientId;
            Reference = reference;
            Ticket = ticket;
        }
        
        
        public string AccountNumber { get;  }
        public DateTime CreatedAt { get;  }
        public string Narration { get;  }
        public decimal Amount { get;  }

        public decimal BalanceBefore { get;  }
        public decimal BalanceAfter { get;  }

        public TransactionType TransactionType { get;  }
        public bool SatisfiedCondition { get;  }

        public string ConditionFailReason { get;  }
        /// <summary>
        /// If honoured, it means outcome was irrespective of  SatisfiedCondition's value
        /// </summary>
        public bool Honoured { get;  }
        public string ClientId { get;  }
        
        public string Reference { get;  }
        
        public string Ticket { get;  }
    }
}