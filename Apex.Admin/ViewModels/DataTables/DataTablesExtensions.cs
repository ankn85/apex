namespace Apex.Admin.ViewModels.DataTables
{
    public static class DataTablesExtensions
    {
        public static DataTablesJsonResult CreateResponse(
            this DataTablesRequest request,
            string errorMessage)
        {
            var response = DataTablesResponse.Create(request, errorMessage);

            return new DataTablesJsonResult(response);
        }

        public static DataTablesJsonResult CreateResponse(
            this DataTablesRequest request,
            int totalRecords,
            int totalRecordsFiltered,
            object data)
        {
            var response = DataTablesResponse.Create(request, totalRecords, totalRecordsFiltered, data);

            return new DataTablesJsonResult(response);
        }
    }
}
