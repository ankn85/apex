using System;
using Apex.Services.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Apex.Admin.ViewModels.DataTables
{
    public class DataTablesRequest
    {
        public int Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public string SortColumnName { get; protected set; }

        public SortDirection SortDirection { get; private set; }

        public virtual void ParseFormData(IFormCollection formData)
        {
            StringValues tempOrder;

            if (formData.TryGetValue("order[0][column]", out tempOrder))
            {
                string columnIndex = tempOrder.ToString();
                SortDirection = GetSortDirection(formData["order[0][dir]"].ToString());

                if (formData.TryGetValue($"columns[{columnIndex}][orderable]", out tempOrder))
                {
                    string columnSortable = tempOrder.ToString();

                    if (columnSortable.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        if (formData.TryGetValue($"columns[{columnIndex}][data]", out tempOrder))
                        {
                            SortColumnName = tempOrder.ToString();
                        }
                    }
                }
            }
        }

        private SortDirection GetSortDirection(string name)
        {
            return (name ?? string.Empty).Equals("desc", StringComparison.OrdinalIgnoreCase)
                ? SortDirection.Descending
                : SortDirection.Ascending;
        }
    }
}
