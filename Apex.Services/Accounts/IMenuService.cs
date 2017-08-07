using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;

namespace Apex.Services.Accounts
{
    public interface IMenuService : IService<Menu>
    {
        Task<IList<Menu>> GetListAsync(ApplicationUser user);

        Task<IList<Menu>> GetReadListAsync(ApplicationUser user);
    }
}
