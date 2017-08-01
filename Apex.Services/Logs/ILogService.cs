using System;
using System.Threading.Tasks;
using Apex.Data.Entities.Logs;
using Apex.Data.Paginations;
using Apex.Services.Enums;
using Apex.Services.Models.Logs;

namespace Apex.Services.Logs
{
    public interface ILogService : IService
    {
        Task<Log> FindAsync(int id);

        Task<IPagedList<LogDto>> GetListAsync(
            DateTime fromDate, 
            DateTime toDate, 
            string level,
            string sortColumnName,
            SortDirection sortDirection,
            int page, 
            int size);

        Task<int> DeleteAsync(int id);

        Task<int> DeleteAsync(int[] ids);
	}
}
