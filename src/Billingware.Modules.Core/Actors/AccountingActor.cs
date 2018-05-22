using System.Data.Entity;
using System.Linq;
using Billingware.Common.Actors;
using Billingware.Models;
using Billingware.Modules.Core.Events;

namespace Billingware.Modules.Core.Actors
{
    public class AccountingActor:BaseActor
    {
        public AccountingActor()
        {
            Receive<DebitAccount>(x => DoDebitAccountEvent(x));
        }

        private void DoDebitAccountEvent(DebitAccount detail)
        {
            using (var db = new BillingwareDataContext())
            {
                var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == detail.Request.AccountNumber);

                if (account==null)
                {
                    return;
                }

                account.Balance -= detail.Request.Amount;

                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}