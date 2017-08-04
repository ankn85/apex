using System.ComponentModel.DataAnnotations;
using Apex.Data.Entities.Emails;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.ViewModels.Emails
{
    public sealed class EmailAccountViewModel
    {
        public EmailAccountViewModel()
        {
        }

        public EmailAccountViewModel(EmailAccount entity)
        {
            Id = entity.Id;
            Email = entity.Email;
            DisplayName = entity.DisplayName;
            Host = entity.Host;
            Port = entity.Port;
            UserName = entity.UserName;
            Password = entity.Password;
            EnableSsl = entity.EnableSsl;
            UseDefaultCredentials = entity.UseDefaultCredentials;
            IsDefaultEmailAccount = entity.IsDefaultEmailAccount;
        }

        [HiddenInput]
        public int Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Display Name")]
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
