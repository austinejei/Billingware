using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billingware.Common.Logging;
using Billingware.Models.Core;

namespace Billingware.Models
{
    public class BillingwareDataContext: DbContext
    {
        public BillingwareDataContext() : base("DefaultConnection")
        {
            //comment this line when you want to do migrations
            //Database.Log += s => { CommonLogger.Info<BillingwareDataContext>(s); };
            // Configuration.ProxyCreationEnabled = false;

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<ConditionOutcome> Outcomes { get; set; }
    }

    public enum TransactionType
    {
        Debit,
        Credit
    }
}
