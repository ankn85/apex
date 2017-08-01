using Apex.Admin.ViewModels.DataTables;
using Microsoft.AspNetCore.Http;

namespace Apex.Admin.ViewModels.Accounts
{
    public sealed class AccountSearchViewModel : DataTablesRequest
    {
        public string Email { get; set; }

        public int[] RoleIds { get; set; }

        public bool Locked { get; set; }

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
