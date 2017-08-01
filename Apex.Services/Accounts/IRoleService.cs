using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;

namespace Apex.Services.Accounts
{
    public interface IRoleService
    {
        Task<IList<ApplicationRole>> GetListAsync();
    }
}
