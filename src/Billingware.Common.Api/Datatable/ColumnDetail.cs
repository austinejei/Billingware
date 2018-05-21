namespace Billingware.Common.Api.Datatable
{
    public class ColumnDetail
    {
        public ColumnDetail()
        {
            Search = new SearchTerm();
        }
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public SearchTerm Search { get; set; }
    }
}