﻿using System;
using System.Collections.Generic;
using System.Linq;
using Apex.Data.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Apex.Data.Configurations
{
    public class EntityFrameworkConfigurationProvider : ConfigurationProvider
    {
        protected Action<DbContextOptionsBuilder> OptionsAction { get; }

        public EntityFrameworkConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<ObjectDbContext>();
            OptionsAction(builder);

            using (var dbContext = new ObjectDbContext(builder.Options))
            {
                dbContext.Database.EnsureCreated();
                var settings = dbContext.Set<Setting>().AsEnumerable();

                if (!settings.Any())
                {
                    settings = SeedSettings(dbContext);
                }

                Data = settings.ToDictionary(s => s.Name, s => s.Value);
            }
        }

        private IEnumerable<Setting> SeedSettings(ObjectDbContext dbContext)
        {
            var configSettings = new List<Setting>
            {
                new Setting { Name = "account.lockoutonfailure", Value = "true" }
            };

            dbContext.Set<Setting>().AddRange(configSettings);

            dbContext.SaveChanges();

            return configSettings;
        }
    }
}
