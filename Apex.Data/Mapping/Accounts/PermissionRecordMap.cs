using Apex.Data.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping.Accounts
{
    public sealed class PermissionRecordMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<PermissionRecord>();

            builder.ToTable("PermissionRecords", "dbo").HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(256);

            builder.Property(p => p.SystemName).IsRequired().HasMaxLength(256);

            builder.Property(p => p.Category).IsRequired().HasMaxLength(256);
        }
    }
}
