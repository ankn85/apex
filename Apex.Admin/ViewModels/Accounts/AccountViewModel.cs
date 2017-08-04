using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apex.Data.Entities.Accounts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Admin.ViewModels.Accounts
{
    public abstract class AccountViewModel
    {
        public AccountViewModel()
        {
        }

        public AccountViewModel(ApplicationUser entity, string[] roleNames)
        {
            Email = entity.Email;
            FullName = entity.FullName;
            Gender = entity.Gender;
            Birthday = entity.Birthday;
            PhoneNumber = entity.PhoneNumber;
            Address = entity.Address;
            Locked = entity.LockoutEnd != null && entity.LockoutEnd.Value > DateTimeOffset.UtcNow;
            Roles = roleNames;
        }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public byte Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public bool Locked { get; set; }

        public string[] Roles { get; set; }

        public IEnumerable<SelectListItem> AllRoles { get; set; }

        public IEnumerable<SelectListItem> AllGenders { get; set; }
    }
}
