using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping
{
    public interface IEntityMapper
    {
        IEnumerable<IEntityMap> Mappings { get; }

        void MapEntities(ModelBuilder modelBuilder);
    }
}
