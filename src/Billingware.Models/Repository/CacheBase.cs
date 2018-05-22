using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;

namespace Billingware.Models.Repository
{
    public abstract class CacheBase
    {
        protected static string GetTableName(Type t)
        {
            var pluralizer = PluralizationService.CreateService(CultureInfo.CurrentCulture);

            var tableName = pluralizer.Pluralize(t.Name);

            return tableName;
        }
    }
}