using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Apex.Services.Constants;

namespace Apex.Admin.ViewModels.Accounts
{
    public sealed class AccountViewModel
    {
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = ValidationRules.MinPasswordLength)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string Gender { get; set; }

        public DateTime? Birthday { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public bool Locked { get; set; }

        public IList<string> Roles { get; set; }
    }
}
