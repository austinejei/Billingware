using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Billingware.Models.Core
{
    public class Transaction
    {
        public int Id { get; set; }
        [Index, StringLength(50)]
        public string AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }

        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }

        public TransactionType TransactionType { get; set; }
        public bool SatisfiedCondition { get; set; }

        public string ConditionFailReason { get; set; }
        /// <summary>
        /// If honoured, it means outcome was irrespective of  SatisfiedCondition's value
        /// </summary>
        public bool Honoured { get; set; }
        public string ClientId { get; set; }
        [Index, StringLength(50)]
        public string Reference { get; set; }
        [Index, StringLength(50)]
        public string Ticket { get; set; }

    }


    public enum ConditionConnector
    {
        And,
        Or,
        None
    }

    public enum OutcomeType
    {
        Proceed,
        Halt
    }


    public enum ComparatorKey
    {
        /// <summary>
        /// Current balance
        /// </summary>
        Balance,
        /// <summary>
        /// Count of all transactions
        /// </summary>
        TransactionsCount,
        /// <summary>
        /// Count of all debit transactions
        /// </summary>
        DebitTransactionsCount,
        /// <summary>
        /// Count of all credit transactions
        /// </summary>
        CreditTransactionsCount,
        /// <summary>
        /// Sum of all tranasactions
        /// </summary>
        TransactionsSum,
        /// <summary>
        /// Sum of debit transaction amounts
        /// </summary>
        DebitTransactionsSum,
        /// <summary>
        /// Sum of all credit transaction amounts
        /// </summary>
        CreditTransactionsSum,
        /// <summary>
        /// Custom expression to be used on KeyExpression
        /// </summary>
        Custom,
    }



    public enum ComparatorType
    {
        GreaterThan,
        LessThan,
        EqualTo,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        Null
    }

    public enum ConditionApplicatorType
    {
        All,
        One
    }

   
}