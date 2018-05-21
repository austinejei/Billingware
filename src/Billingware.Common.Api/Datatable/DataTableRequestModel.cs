using System.Collections.Generic;

namespace Billingware.Common.Api.Datatable
{
    public class DataTableRequestModel
    {
        public DataTableRequestModel()
        {
            Order = new List<OrderDetails>();
            Search = new SearchTerm();
        }
        public int Draw { get; set; }
        public int Start { get; set; }

        public int Length { get; set; }

        public SearchTerm Search { get; set; }
        public List<OrderDetails> Order { get; set; }

        public List<ColumnDetail> Columns { get; set; }
    }
}