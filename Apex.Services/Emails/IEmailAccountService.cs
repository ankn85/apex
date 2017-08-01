using System.Threading.Tasks;
using Apex.Data.Entities.Emails;
using Apex.Data.Paginations;
using Apex.Services.Enums;

namespace Apex.Services.Emails
{
    public interface IEmailAccountService
    {
        Task<EmailAccount> FindAsync(int id);

        Task<IPagedList<EmailAccount>> GetListAsync(
            string sortColumnName,
            SortDirection sortDirection);

        Task<EmailAccount> GetDefaultAsync();

        Task<EmailAccount> CreateAsync(EmailAccount entity);

        Task<int> UpdateAsync(EmailAccount entity);

        Task<int> DeleteAsync(int id);

        Task<int> DeleteAsync(int[] ids);
    }
}
