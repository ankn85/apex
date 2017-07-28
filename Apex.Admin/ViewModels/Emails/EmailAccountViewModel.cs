using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.ViewModels.Emails
{
    public sealed class EmailAccountViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string DisplayName { get; set; }

        [Required]
        public string Host { get; set; }

        [Range(1, ushort.MaxValue)]
        public int Port { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public bool IsDefaultEmailAccount { get; set; }
    }
}
