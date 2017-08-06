using System;
using System.Threading.Tasks;
using Apex.Data.Entities.Logs;
using Apex.Data.Paginations;
using Apex.Data.Sorts;
using Apex.Services.Models.Logs;

namespace Apex.Services.Logs
{
    public interface ILogService : IService<Log>
    {
        Task<IPagedList<LogDto>> GetListAsync(
            DateTime fromDate, 
            DateTime toDate,
            string[] levels,
            string sortColumnName,
            SortDirection sortDirection,
            int page, 
            int size);
	}
}
