using Apex.Data.Entities.Accounts;
using Apex.Services.Models.Accounts;
using System.Threading.Tasks;

namespace Apex.Services.Accounts
{
    public interface IACLService : IService<ApplicationRoleMenu>
    {
        Task<ACLDto> GetListAsync();
    }
}
