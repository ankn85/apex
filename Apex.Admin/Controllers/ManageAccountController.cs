using System;
using System.Linq;
using System.Threading.Tasks;
using Apex.Admin.ViewModels.Accounts;
using Apex.Admin.ViewModels.DataTables;
using Apex.Data.Entities.Accounts;
using Apex.Services.Accounts;
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
            await PopularRoleIds();

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
            await PopularRoleNames();

            return View(new AccountViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser entity = ParseApplicationUser(model);
                await _accountService.CreateAsync(entity, model.Password, model.Locked, model.Roles);

                return RedirectToAction(nameof(Index));
            }

            await PopularRoleNames();

            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            ApplicationUser entity = await _accountService.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }


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
        private async Task PopularRoleIds()
        {
            var roles = await _roleService.GetListAsync();

            ViewData["Roles"] = roles.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name });
        }

        [NonAction]
        private async Task PopularRoleNames()
        {
            var roles = await _roleService.GetListAsync();

            ViewData["Roles"] = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });
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
                EmailConfirmed = true
            };
        }

        [NonAction]
        private AccountViewModel ParseAccountViewModel(ApplicationUser entity)
        {
            return new AccountViewModel
            {
                Id = entity.Id,
                Email = entity.Email,
                Locked = entity.LockoutEnd != null && entity.LockoutEnd.Value > DateTimeOffset.UtcNow
            };
        }
    }
}
