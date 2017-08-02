using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Admin.ViewModels.Accounts;
using Apex.Admin.ViewModels.DataTables;
using Apex.Data.Entities.Accounts;
using Apex.Services.Accounts;
using Apex.Services.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Admin.Controllers
{
    public class ManageAccountController : AdminController
    {
        private readonly IRoleService _roleService;
        private readonly IAccountService _accountService;

        public ManageAccountController(
            IRoleService roleService,
            IAccountService accountService)
        {
            _roleService = roleService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            await PopularRoles(false);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(AccountSearchViewModel model)
        {
            model.ParseFormData(Request.Form);

            var logs = await _accountService.GetListAsync(
                model.Email,
                model.RoleIds,
                model.SortColumnName,
                model.SortDirection,
                model.Start,
                model.Length);

            return model.CreateResponse(logs.TotalRecords, logs.TotalRecordsFiltered, logs);
        }

        public async Task<IActionResult> Create()
        {
            await PopularRoles();

            return View(new AccountViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser entity = ParseApplicationUser(model);
                var result = await _accountService.CreateAsync(entity, model.Password, model.Locked, model.Roles);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                AddErrorsToModelState(result);
            }

            await PopularRoles();

            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            ApplicationUser entity = await _accountService.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            IList<string> roleNames = await _accountService.GetRolesAsync(entity);
            AccountViewModel model = ParseAccountViewModel(entity, roleNames);

            await PopularRoles();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int[] ids)
        {
            int idsCount = ids != null ? ids.Length : 0;

            if (idsCount == 0)
            {
                return BadRequestApiError("ApplicationUserId", "'ApplicationUser Ids' should not be empty.");
            }

            int effectedRows = 0;
            IdentityResult result;

            foreach (int id in ids)
            {
                result = await _accountService.DeleteAsync(id);

                if (result.Succeeded)
                {
                    effectedRows++;
                }
            }

            return Ok(effectedRows);
        }

        [NonAction]
        private async Task PopularRoles(bool valueIsName = true)
        {
            var roles = await _roleService.GetListAsync();

            IEnumerable<SelectListItem> selectListItems = valueIsName
                ? roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name })
                : roles.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name });

            ViewData["Roles"] = selectListItems;
        }

        [NonAction]
        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [NonAction]
        private ApplicationUser ParseApplicationUser(AccountViewModel model)
        {
            string email = model.Email.Trim();

            return new ApplicationUser
            {
                Id = model.Id,
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = model.FullName.Trim(),
                Gender = model.Gender.TrimNull(),
                Birthday = model.Birthday,
                PhoneNumber = model.PhoneNumber.TrimNull(),
                Address = model.Address.TrimNull()
            };
        }

        [NonAction]
        private AccountViewModel ParseAccountViewModel(ApplicationUser entity, IList<string> roleNames)
        {
            return new AccountViewModel
            {
                Id = entity.Id,
                Email = entity.Email,
                Password = string.Empty,
                FullName = entity.FullName,
                Gender = entity.Gender,
                Birthday = entity.Birthday,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                Locked = entity.LockoutEnd != null && entity.LockoutEnd.Value > DateTimeOffset.UtcNow,
                Roles = roleNames
            };
        }
    }
}
