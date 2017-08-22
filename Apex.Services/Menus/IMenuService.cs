using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Data.Entities.Menus;

namespace Apex.Services.Menus
{
    public interface IMenuService : IService<Menu>
    {
        Task<IList<Menu>> GetListAsync();

        Task<IList<Menu>> GetReadListAsync(IEnumerable<int> roleIds);
    }
}
