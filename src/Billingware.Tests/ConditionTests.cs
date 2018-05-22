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
            var condition = new Condition
            {
                Active = true,
                ConditionApplicatorType = ConditionApplicatorType.All,
                ConditionConnector = ConditionConnector.None,
                Key = ComparatorKey.Balance,
                Type = ComparatorType.LessThanOrEqualTo,
                Name = "halt_no_balance",
                Value = "0",
                OutcomeId = 1
            };

            var outcome = new ConditionOutcome
            {
                Active = true,
                OutcomeType = OutcomeType.Halt
            };



            //act
            var db = new BillingwareDataContext();
            db.Conditions.Add(condition);
            db.Outcomes.Add(outcome);
            db.SaveChanges();

            //assert
            Assert.True(condition.Id>0);
        }
    }
}