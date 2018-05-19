namespace Billingware.Models.Core
{
    public class Condition
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public ConditionApplicatorType ConditionApplicatorType { get; set; }
        public string AppliedToAccountNumbers { get; set; }
        public ComparatorKey Key { get; set; }
        public string KeyExpression { get; set; }
        public ComparatorType Type { get; set; }
        public string Value { get; set; }

        public int? OutcomeId { get; set; }
        public bool Active { get; set; }
        public ConditionConnector ConditionConnector { get; set; }

    }
}