using System;
using System.Collections.Generic;
using System.Linq;
using Apex.Data.Entities.Accounts;
using Apex.Services.Enums;
using Apex.Services.Extensions;

namespace Apex.Services.Models.Accounts
{
    public sealed class ApplicationUserDto
    {
        public ApplicationUserDto(ApplicationUser entity, IDictionary<int, string> roleHash)
        {
            Id = entity.Id;
            Email = entity.Email;
            EmailConfirmed = entity.EmailConfirmed;
            AccessFailedCount = entity.AccessFailedCount;
            Roles = string.Join(", ", entity.Roles.Select(r => roleHash[r.RoleId]));

            if (entity.LockoutEnabled && entity.LockoutEnd.HasValue)
            {
                Locked = entity.LockoutEnd.Value >= DateTimeOffset.UtcNow;
            }
            else
            {
                Locked = false;
            }

            FullName = entity.FullName;
            Gender = ((Gender)entity.Gender).ToString();

            if (entity.Birthday != null)
            {
                Birthday = entity.Birthday.Value.ToDateString();
            }

            PhoneNumber = entity.PhoneNumber;
            Address = entity.Address;
        }

        public int Id { get; set; }

        public string Email { get; }

        public bool EmailConfirmed { get; }

        public int AccessFailedCount { get; }

        public string Roles { get; }

        public bool Locked { get; }

        public string FullName { get; }

        public string Gender { get; }

        public string Birthday { get; }

        public string PhoneNumber { get; }

        public string Address { get; set; }
    }
}
