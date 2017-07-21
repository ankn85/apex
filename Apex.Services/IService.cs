using System.Threading.Tasks;

namespace Apex.Services
{
    public interface IService
    {
        Task<int> CommitAsync();
    }
}
