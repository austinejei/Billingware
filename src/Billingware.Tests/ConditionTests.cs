using Billingware.Models;
using Billingware.Models.Core;
using Xunit;

namespace Billingware.Tests
{
    public class ConditionTests
    {
        [Fact]
        public void CreateSampleCondition()
        {
            //arrange

            //create condition for the following case:
            //when the request is "debit" and the account balance <=0, halt the process
            var whenRequestIsDebit = new Condition
            {
                Active = true,
                ConditionApplicatorType = ConditionApplicatorType.All,
                ConditionConnector = ConditionConnector.And,
                Key = ComparatorKey.Custom,
                KeyExpression = "$.Payload.TransactionType",
                Type = ComparatorType.EqualTo,
                Name = "halt_no_balance",
                Value = "debit",
                OutcomeId = 1 //same outcome
            };
            var whenAccountHasNoMoney = new Condition
            {
                Active = true,
                ConditionApplicatorType = ConditionApplicatorType.All,
                ConditionConnector = ConditionConnector.None,
                Key = ComparatorKey.Balance,
                Type = ComparatorType.LessThanOrEqualTo,
                Name = "halt_no_balance",
                Value = "0",
                OutcomeId = 1 //same outcome
            };

            //when account has "allowOverdraft=true" in their Extras 

            var whenAccountHasAllowOverdraftFlag = new Condition
            {
                Active = true,
                ConditionApplicatorType = ConditionApplicatorType.One,
                AppliedToAccountNumbers= "9237452718263673",
                ConditionConnector = ConditionConnector.None,
                Key = ComparatorKey.Custom,
                KeyExpression = "$.Extra.allowOverdraft",
                Type = ComparatorType.EqualTo,
                Name = "halt_no_balance",
                Value = "true",
                OutcomeId = 1 //same outcome
            };

            var outcome = new ConditionOutcome
            {
                Active = true,
                OutcomeType = OutcomeType.Halt
            };



            //act
            var db = new BillingwareDataContext();
            db.Conditions.Add(whenRequestIsDebit);
            db.Conditions.Add(whenAccountHasNoMoney);
            db.Conditions.Add(whenAccountHasAllowOverdraftFlag);
            db.Outcomes.Add(outcome);
            db.SaveChanges();

            //assert
            Assert.True(whenRequestIsDebit.Id>0);
        }
    }
}