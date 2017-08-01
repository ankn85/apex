using System;
using System.Collections.Generic;
using System.Linq;

namespace Apex.Services.Models.Accounts
{
    public sealed class ApplicationUserDto
    {
        public ApplicationUserDto(
            int id,
            string email,
            string name,
            IEnumerable<int> roles,
            bool emailConfirmed,
            bool lockoutEnabled,
            DateTimeOffset? lockoutEnd,
            int accessFailedCount,
            bool twoFactorEnabled,
            bool phoneNumberConfirmed,
            string phoneNumber,
            IDictionary<int, string> roleHash)
        {
            Id = id;
            Email = email;
            Name = name;
            Roles = string.Join(", ", roles.Select(r => roleHash[r]));
            EmailConfirmed = emailConfirmed;

            if (lockoutEnabled && lockoutEnd.HasValue)
            {
                Locked = lockoutEnd.Value >= DateTimeOffset.UtcNow;
            }
            else
            {
                Locked = false;
            }

            AccessFailedCount = accessFailedCount;
            TwoFactorEnabled = twoFactorEnabled;
            PhoneNumberConfirmed = phoneNumberConfirmed;
            PhoneNumber = phoneNumber;
        }

        public int Id { get; set; }

        public string Email { get; }

        public string Name { get; }

        public string Roles { get; }

        public bool EmailConfirmed { get; }

        public bool Locked { get; }

        public int AccessFailedCount { get; }

        public bool TwoFactorEnabled { get; }

        public bool PhoneNumberConfirmed { get; }

        public string PhoneNumber { get; }
    }
}
