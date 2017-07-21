using Apex.Admin.ViewModels.DataTables;
using Microsoft.AspNetCore.Http;

namespace Apex.Admin.ViewModels.Emails
{
    public sealed class EmailAccountSearchViewModel : DataTablesRequest
    {
        public override void ParseFormData(IFormCollection formData)
        {
            base.ParseFormData(formData);

            if (string.IsNullOrEmpty(SortColumnName))
            {
                SortColumnName = "email";
            }
        }
    }
}
