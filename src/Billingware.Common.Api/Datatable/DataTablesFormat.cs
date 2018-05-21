

//using PetaPoco;
//using Transaction = MySierraBox.DataLayer.Transaction;


namespace Billingware.Common.Api.Datatable
{
    public class DataTablesFormat
    {
      

        public static object PageResponseForTransactions(DataTableRequestModel pageRequestModel,int total,int totalFiltered, object data)
        {
            return new
            {
                draw = pageRequestModel.Draw,
                recordsTotal = total,
                recordsFiltered = total,
              
                data
            };
        }


    }
}