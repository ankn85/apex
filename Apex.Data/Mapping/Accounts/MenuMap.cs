using Apex.Data.Entities.Accounts;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping.Accounts
{
    public sealed class MenuMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<Menu>();

            builder.ToTable("Menus", "dbo").HasKey(p => p.Id);

            builder.Property(p => p.Title).IsRequired().HasMaxLength(256);

            builder.Property(p => p.Description).HasMaxLength(512);

            builder.Property(p => p.Url).HasMaxLength(256);

            builder.Property(p => p.Url).HasMaxLength(128);

            builder
                .HasMany(m => m.SubMenus)
                .WithOne(m => m.ParentMenu)
                .HasForeignKey(m => m.ParentId);
        }
    }
}
