﻿using Apex.Data.Entities.Emails;
using Microsoft.EntityFrameworkCore;

namespace Apex.Data.Mapping.Emails
{
    public sealed class QueuedEmailMap : IEntityMap
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<QueuedEmail>();

            builder.ToTable("QueuedEmails", "dbo").HasKey(p => p.Id);

            builder.Property(p => p.From).IsRequired().HasMaxLength(256);

            builder.Property(p => p.FromName).HasMaxLength(256);

            builder.Property(p => p.To).IsRequired().HasMaxLength(256);

            builder.Property(p => p.ToName).HasMaxLength(256);

            builder.Property(p => p.ReplyTo).HasMaxLength(256);

            builder.Property(p => p.ReplyToName).HasMaxLength(256);

            builder.Property(p => p.Subject).IsRequired().HasMaxLength(256);

            builder.Property(p => p.Body).IsRequired();

            builder.HasOne(qe => qe.EmailAccount)
                .WithMany(ea => ea.QueuedEmails)
                .HasForeignKey(qe => qe.EmailAccountId);
        }
    }
}
