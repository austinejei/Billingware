using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Billingware.Models.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Billingware.Modules.Core.Helpers
{
    public class ConditionEvaluatorHelper
    {
        public static List<int> EvaluateConditionAndGetOutcomeIds(Account account,List<Condition> conditions,ClientPayloadData payloadData, decimal sumOfCreditTransactions = 0, long countOfCreditTransactions = 0, decimal sumOfDebitTransactions = 0, long countOfDebitTransactions = 0, long countOfTransactions = 0)
        {
            //Log(CommonLogLevel.Info, $"received request to {nameof(DoGetCount)} for {_tableName}", null);
            try
            {
                //bool conditionResult;


                //we have to group the conditions in steps that belong to the same outcome
                //var conditions = activity.Conditions.Where(c => c.Active).ToList();


                var steps = conditions.GroupBy(k => k.OutcomeId);

                var stepResults = new List<Tuple<bool, int>>();
                foreach (var step in steps)
                {
                    var stepConditions = step.Where(c => c.Active);

                    var exeResult = EvaluateConditions(account,stepConditions.ToList(),payloadData,sumOfCreditTransactions,countOfCreditTransactions,sumOfDebitTransactions,countOfDebitTransactions,countOfTransactions);

                    stepResults.Add(new Tuple<bool, int>(exeResult,
                        step.Key.Value));

                }

                var res = stepResults.Where(s => s.Item1);

                if (!res.Any())
                {
                    return new List<int>();
                }

                return res.Select(r => r.Item2).ToList();
            }
            catch (Exception e) { return new List<int>(); }
        }


        private static bool EvaluateConditions(Account account,List<Condition> conditions, ClientPayloadData payloadData, decimal sumOfCreditTransactions = 0, long countOfCreditTransactions = 0, decimal sumOfDebitTransactions = 0, long countOfDebitTransactions = 0, long countOfTransactions = 0)
        {
            bool conditionResult;

            ConditionConnector previousOperator = ConditionConnector.None;

            Expression previousExpression = null;
            Expression finalExpression = null;

            foreach (var condition in conditions)
            {

                //var activityConditionComparatorValue = JsonConvert.DeserializeObject<ActivityConditionComparatorValue>(condition.ConditionValue);

                object activityConditionKey;
                switch (condition.Key)
                {
                    case  ComparatorKey.Balance:
                        
                        activityConditionKey = account.Balance;
                        break;
                    case ComparatorKey.CreditTransactionsCount:
                        
                        activityConditionKey = countOfCreditTransactions;
                        break;
                    case ComparatorKey.CreditTransactionsSum:

                        activityConditionKey = sumOfCreditTransactions;
                        break;
                    case ComparatorKey.DebitTransactionsCount:
                        activityConditionKey = countOfDebitTransactions;
                        break;
                    case ComparatorKey.DebitTransactionsSum:

                        activityConditionKey = sumOfDebitTransactions;
                        break;
                    case ComparatorKey.TransactionsCount:
                        activityConditionKey = countOfTransactions;
                        break;
                    case ComparatorKey.Custom:
                        //pick from Extras on Account object
                        var v = GetCustomExpressionValue(account,payloadData,
                            condition.KeyExpression);

                        
                        activityConditionKey = v;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                //ConstantExpression key = Expression.Constant(activityConditionKey);

                //Expression leftExpression = Expression.Property(cachedCopy,"Item",key);
                Expression leftExpression = Expression.Constant(activityConditionKey,
                    activityConditionKey.GetType());

                //leftExpression = Expression.Convert(leftExpression,activityConditionComparatorValue.Value.GetType());
                
                switch (condition.Type)
                {
                    case ComparatorType.EqualTo:


                        Expression eqleft = leftExpression;

                        Expression eqright = Expression.Constant(condition.Value,
                            condition.Value.GetType());

                        Expression eqResult = Expression.Equal(eqleft,
                            eqright);

                        if (previousOperator == ConditionConnector.And)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.And(finalExpression,
                                    eqResult);
                            }
                            else
                            {
                                finalExpression = Expression.And(previousExpression,
                                    eqResult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.Or)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.Or(finalExpression,
                                    eqResult);
                            }
                            else
                            {
                                finalExpression = Expression.Or(previousExpression,
                                    eqResult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.None)
                        {
                            if (finalExpression != null) { }
                            else { finalExpression = eqResult; }
                        }
                        previousExpression = eqResult;
                        break;
                    case ComparatorType.GreaterThan:

                        //Expression gtleft = Expression.PropertyOrField(cachedCopy, whereClauseItem.Field);
                        Expression gtleft = leftExpression;
                        Expression gtright = Expression.Constant(condition.Value,
                            condition.Value.GetType());
                        Expression gtresult = Expression.GreaterThan(gtleft,
                            gtright);

                        if (previousOperator == ConditionConnector.And)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.And(finalExpression,
                                    gtresult);
                            }
                            else
                            {
                                finalExpression = Expression.And(previousExpression,
                                    gtresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.Or)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.Or(finalExpression,
                                    gtresult);
                            }
                            else
                            {
                                finalExpression = Expression.Or(previousExpression,
                                    gtresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.None)
                        {
                            if (finalExpression != null) { }
                            else { finalExpression = gtresult; }
                        }

                        previousExpression = gtresult;
                        break;
                    case ComparatorType.LessThan:

                        //Expression ltleft = Expression.PropertyOrField(cachedCopy, whereClauseItem.Field);
                        Expression ltleft = leftExpression;
                        Expression ltright = Expression.Constant(condition.Value,
                            condition.Value.GetType());
                        Expression ltresult = Expression.LessThan(ltleft,
                            ltright);

                        if (previousOperator == ConditionConnector.And)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.And(finalExpression,
                                    ltresult);
                            }
                            else
                            {
                                finalExpression = Expression.And(previousExpression,
                                    ltresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.Or)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.Or(finalExpression,
                                    ltresult);
                            }
                            else
                            {
                                finalExpression = Expression.Or(previousExpression,
                                    ltresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.None)
                        {
                            if (finalExpression != null) { }
                            else { finalExpression = ltresult; }
                        }
                        previousExpression = ltresult;
                        break;
                    case ComparatorType.GreaterThanOrEqualTo:
                        //Expression gtoreqleft = Expression.PropertyOrField(cachedCopy, whereClauseItem.Field);
                        Expression gtoreqleft = leftExpression;
                        Expression gtoreqright = Expression.Constant(condition.Value,
                            condition.Value.GetType());
                        Expression gtoreqresult = Expression.LessThanOrEqual(gtoreqleft,
                            gtoreqright);

                        if (previousOperator == ConditionConnector.And)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.And(finalExpression,
                                    gtoreqresult);
                            }
                            else
                            {
                                finalExpression = Expression.And(previousExpression,
                                    gtoreqresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.Or)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.Or(finalExpression,
                                    gtoreqresult);
                            }
                            else
                            {
                                finalExpression = Expression.Or(previousExpression,
                                    gtoreqresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.None)
                        {
                            if (finalExpression != null) { }
                            else { finalExpression = gtoreqresult; }
                        }

                        previousExpression = gtoreqresult;
                        break;
                    case ComparatorType.LessThanOrEqualTo:

                        // Expression ltoreqleft = Expression.PropertyOrField(cachedCopy, whereClauseItem.Field);
                        Expression ltoreqleft = leftExpression;
                        Expression ltoreqright = Expression.Constant(condition.Value,
                            condition.Value.GetType());
                        Expression ltoreqresult = Expression.LessThanOrEqual(ltoreqleft,
                            ltoreqright);

                        if (previousOperator == ConditionConnector.And)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.And(finalExpression,
                                    ltoreqresult);
                            }
                            else
                            {
                                finalExpression = Expression.And(previousExpression,
                                    ltoreqresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.Or)
                        {
                            if (finalExpression != null)
                            {
                                finalExpression = Expression.Or(finalExpression,
                                    ltoreqresult);
                            }
                            else
                            {
                                finalExpression = Expression.Or(previousExpression,
                                    ltoreqresult);
                            }
                        }
                        else if (previousOperator == ConditionConnector.None)
                        {
                            if (finalExpression != null) { }
                            else { finalExpression = ltoreqresult; }
                        }
                        previousExpression = ltoreqresult;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                previousOperator = condition.ConditionConnector;
            }



            if (finalExpression != null)
            {

                var t = Expression.Condition(finalExpression,
                    Expression.Constant(true),
                    Expression.Constant(false));

                conditionResult = Expression.Lambda<Func<decimal, bool>>(t, new ParameterExpression[] { Expression.Parameter(typeof(decimal)) }).Compile()(decimal.Zero);


            }
            else { conditionResult = false; }



            return conditionResult;
        }


        private static object GetCustomExpressionValue(Account account,ClientPayloadData payloadData,
            string keyExpression)
        {

            //e.g. keyExpression can be $.Extra.active or $.Balance or $.Payload.Amount, etc
            JObject extraJson = null;
            if (!string.IsNullOrEmpty(account.Extra))
            {
               extraJson = JObject.Parse(account.Extra);
            }

            var json = JObject.Parse(JsonConvert.SerializeObject(new
            {
                account.AccountNumber,
                account.Balance,
                account.Alias,
                account.CreatedAt,
                Extra = extraJson,
                Payload = payloadData
            }));
            
            var result = json.SelectToken(keyExpression);

            return result.ToObject<object>();

        }
    }

    public class ClientPayloadData
    {
        public string TransactionType { get; }
        public string Reference { get; }
        public decimal Amount { get; }

        public ClientPayloadData(string transactionType,string reference,decimal amount)
        {
            TransactionType = transactionType;
            Reference = reference;
            Amount = amount;
        }
    }
}