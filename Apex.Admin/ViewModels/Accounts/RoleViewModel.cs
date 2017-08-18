using System.ComponentModel.DataAnnotations;
using Apex.Data.Entities.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.ViewModels.Accounts
{
    public sealed class RoleViewModel
    {
        public RoleViewModel()
        {
        }

        public RoleViewModel(ApplicationRole entity)
        {
            Id = entity.Id;
            RoleName = entity.Name;
        }

        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
