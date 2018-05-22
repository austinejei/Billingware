using System;
using System.Data.Entity;
using System.Linq;
using Billingware.Common.Actors;
using Billingware.Common.Actors.Messages;
using Billingware.Models;
using Billingware.Models.Core;

namespace Billingware.Modules.Core.Actors
{
    public class AccountManagerActor : BaseActor
    {
        public AccountManagerActor()
        {
            Receive<CreateAccount>(x => DoCreateAccount(x));
            Receive<EditAccount>(x => DoEditAccount(x));
        }

        private void DoEditAccount(EditAccount req)
        {
            var db = new BillingwareDataContext();

            var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == req.AccountNumber);

            if (account==null)
            {
                Sender.Tell(new AccountEdited(new CommonStatusResponse(message: "Not found",code:"404",subCode:"404.1")), Self);
                return;
            }

            account.Alias = req.Alias;
            account.Extra = req.Extra;

            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();

            Sender.Tell(new AccountEdited(new CommonStatusResponse(message: "Successful")), Self);
        }

        private void DoCreateAccount(CreateAccount req)
        {
            var db = new BillingwareDataContext();

            db.Accounts.Add(new Account
            {
                AccountNumber = req.AccountNumber,
                Balance = decimal.Zero,
                Extra = req.Extra,
                Alias = req.Alias,
                CreatedAt = DateTime.Now
            });

            db.SaveChanges();


            Sender.Tell(new AccountCreated(new CommonStatusResponse(message: "Successful")), Self);
        }
    }
}