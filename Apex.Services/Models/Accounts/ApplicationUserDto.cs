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
            bool emailConfirmed,
            int accessFailedCount,
            IEnumerable<int> roles,
            bool lockoutEnabled,
            DateTimeOffset? lockoutEnd,
            string fullName,
            string gender,
            DateTime? birthday,
            string phoneNumber,
            string address,
            IDictionary<int, string> roleHash)
        {
            Id = id;
            Email = email;
            EmailConfirmed = emailConfirmed;
            AccessFailedCount = accessFailedCount;
            Roles = string.Join(", ", roles.Select(r => roleHash[r]));

            if (lockoutEnabled && lockoutEnd.HasValue)
            {
                Locked = lockoutEnd.Value >= DateTimeOffset.UtcNow;
            }
            else
            {
                Locked = false;
            }

            FullName = fullName;
            Gender = gender;
            Birthday = birthday;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public int Id { get; set; }

        public string Email { get; }

        public bool EmailConfirmed { get; }

        public int AccessFailedCount { get; }

        public string Roles { get; }

        public bool Locked { get; }

        public string FullName { get; }

        public string Gender { get; }

        public DateTime? Birthday { get; }

        public string PhoneNumber { get; }

        public string Address { get; set; }
    }
}
