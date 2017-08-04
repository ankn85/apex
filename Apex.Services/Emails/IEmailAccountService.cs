using System.Threading.Tasks;
using Apex.Data.Entities.Emails;
using Apex.Data.Paginations;
using Apex.Data.Sorts;

namespace Apex.Services.Emails
{
    public interface IEmailAccountService : IService
    {
        Task<IPagedList<EmailAccount>> GetListAsync(
            string sortColumnName,
            SortDirection sortDirection);

        Task<EmailAccount> GetDefaultAsync();
    }
}
