using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Billingware.Common.Api.Datatable
{
    public static class DataTableLinqExpressions
    {
        public static IQueryable<T> GetExpression<T>(this DataTableRequestModel pageRequestModel,IQueryable<T> query) where T:class
        {
            ParameterExpression item = Expression.Parameter(typeof(T), "item");
            Expression previousExpression = null;

            Expression finalExpression = null;

            IQueryable<T> results = null;

            if (!string.IsNullOrEmpty(pageRequestModel.Search.Value))
            {
                foreach (var column in pageRequestModel.Columns)
                {
                    if (!column.Searchable) { continue; }

                    if (string.IsNullOrEmpty(column.Name)) { continue; }
                    
                    var property = Expression.Property(item,
                        column.Data);
                    
                    
                    Expression leftExpression = Expression.Property(item,
                        column.Data);

                    
                    //should not be null because we're going to do a contains..
                    bool typeIsString = false;
                    Expression likeneqResult = leftExpression;
                    if (property.Type==typeof(string))
                    {
                        Expression likeneqleft = leftExpression;
                        Expression likeneqright = Expression.Constant(null);
                         likeneqResult = Expression.NotEqual(likeneqleft,
                            likeneqright);

                        typeIsString = true;
                    }
                   

                    //convert to string
                    var toString = typeof(Object).GetMethod("ToString");
                    var toStringValue = Expression.Call(leftExpression, toString);

                    //then we do a contains with IndexOf
                   var indexOf = Expression.Call(toStringValue, "IndexOf", null,
                        Expression.Constant(pageRequestModel.Search.Value, typeof(string)),
                        Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
                    var like = Expression.GreaterThanOrEqual(indexOf, Expression.Constant(0));

                    //we check that the field is not null AND it contains
                    if (!typeIsString)
                    {
                        Expression likeneqright = Expression.Constant(null);
                        likeneqResult = Expression.NotEqual(toStringValue,
                            likeneqright);
                    }
                    

                    Expression likeFinalExpress = Expression.AndAlso(likeneqResult,
                        like);
                    if (previousExpression == null)
                    {
                        finalExpression = likeFinalExpress;
                    }
                    else
                    {
                        finalExpression = Expression.Or(finalExpression,
                            likeFinalExpress);
                    }

                    previousExpression = likeFinalExpress;
                }


                if (finalExpression != null)
                {
                    MethodCallExpression whereCallExpression = Expression.Call(typeof(Queryable),
                        "Where",
                        new Type[]
                        {
                            query.ElementType
                        },
                        query.Expression,
                        Expression.Lambda<Func<T, bool>>(finalExpression,
                            new ParameterExpression[]
                            {
                                item
                            }));

                    results = query.Provider.CreateQuery<T>(whereCallExpression);
                }
                else { results = query; }
            }
            else { results = query; }

            if (pageRequestModel.Order.Any())
            {
                var order = pageRequestModel.Order[0];

                var column = pageRequestModel.Columns[order.Column];

                if (column.Orderable)
                {
                    if (!string.IsNullOrEmpty(column.Name))
                    {
                        var type = typeof(T);
                        var property = type.GetProperty(column.Data);
                        var parameter = Expression.Parameter(type, "p");
                        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                        var orderByExp = Expression.Lambda(propertyAccess, parameter);
                        var typeArguments = new Type[] { type, property.PropertyType };
                        var methodName = ("asc".Equals(order.Dir, StringComparison.CurrentCultureIgnoreCase)) ? "OrderBy" : "OrderByDescending";
                        var resultExp = Expression.Call(typeof(Queryable), methodName, typeArguments, results.Expression, Expression.Quote(orderByExp));

                        results = results.Provider.CreateQuery<T>(resultExp);
                    }
                }

               

              
            }

            //var startPage = (pageRequestModel.Length == 0) ? 1 : pageRequestModel.Start / pageRequestModel.Length + 1;

            //return results.Skip((startPage - 1) * pageRequestModel.Length).Take(pageRequestModel.Length);

            return results;
        }


        private static IEnumerable<string> GetSearchClause(DataTableRequestModel pageRequestModel)
        {

            foreach (var tableColumn in pageRequestModel.Columns)
            {
                if (tableColumn.Searchable)
                {
                    yield return string.Format("{0} LIKE @0", tableColumn.Data);
                }
            }


        }

        private static IEnumerable<string> GetOrderByClause(DataTableRequestModel pageRequestModel)
        {

            foreach (var detail in pageRequestModel.Order)
            {

                yield return string.Format("{0} {1}", pageRequestModel.Columns[detail.Column].Data, detail.Dir);

            }

        }
    }
}