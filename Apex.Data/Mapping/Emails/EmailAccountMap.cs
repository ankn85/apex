using Apex.Data.Entities.Emails;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping.Emails
{
    public sealed class EmailAccountMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<EmailAccount>();

            builder.ToTable("EmailAccounts", "dbo").HasKey(p => p.Id);

            builder.Property(p => p.Email).IsRequired().HasMaxLength(256);

            builder.Property(p => p.DisplayName).HasMaxLength(256);

            builder.Property(p => p.Host).IsRequired().HasMaxLength(256);

            builder.Property(p => p.UserName).IsRequired().HasMaxLength(256);

            builder.Property(p => p.Password).IsRequired().HasMaxLength(256);
        }
    }
}
