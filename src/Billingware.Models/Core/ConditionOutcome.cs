namespace Billingware.Models.Core
{
    public class ConditionOutcome
    {
        public int Id { get; set; }
        public OutcomeType OutcomeType { get; set; }
        public bool Active { get; set; }

    }
}