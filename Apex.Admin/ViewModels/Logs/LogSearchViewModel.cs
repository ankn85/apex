using System;
using Apex.Admin.ViewModels.DataTables;
using Microsoft.AspNetCore.Http;

namespace Apex.Admin.ViewModels.Logs
{
    public sealed class LogSearchViewModel : DataTablesRequest
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string[] Levels { get; set; }

        public override void ParseFormData(IFormCollection formData)
        {
            base.ParseFormData(formData);

            if (string.IsNullOrEmpty(SortColumnName))
            {
                SortColumnName = "logged";
            }

            if (ToDate == DateTime.MinValue)
            {
                ToDate = DateTime.UtcNow;
            }

            if (FromDate == DateTime.MinValue)
            {
                FromDate = ToDate.AddDays(-7);
            }
        }
    }
}
