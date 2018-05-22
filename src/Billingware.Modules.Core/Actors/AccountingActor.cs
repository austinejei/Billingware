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
            Receive<DebitAccountEvent>(x => DoDebitAccountEvent(x));
        }

        private void DoDebitAccountEvent(DebitAccountEvent detail)
        {
            using (var db = new BillingwareDataContext())
            {
                var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == detail.Request.AccountNumber);

                if (account==null)
                {
                    //todo: do we retry??
                    return;
                }

                var balanceBefore = account.Balance;
                account.Balance -= detail.Request.Amount;
                var balanceAfter = account.Balance;

                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}