using System.ComponentModel.DataAnnotations;
using Apex.Data.Entities.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.ViewModels.Accounts
{
    public sealed class AccountUpdateViewModel : AccountViewModel
    {
        public AccountUpdateViewModel()
            : base()
        {
        }

        public AccountUpdateViewModel(ApplicationUser entity, string[] roleNames)
            : base(entity, roleNames)
        {
            Id = entity.Id;
            Password = string.Empty;
        }

        [HiddenInput]
        public int Id { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
