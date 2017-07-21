using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping
{
    public interface IEntityMap
    {
        void Map(ModelBuilder modelBuilder);
    }
}
