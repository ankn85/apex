using System;
using System.Linq;
using System.Threading.Tasks;
using Apex.Admin.ViewModels.Accounts;
using Apex.Admin.ViewModels.DataTables;
using Apex.Data.Entities.Accounts;
using Apex.Services.Accounts;
using Apex.Services.Constants;
using Apex.Services.Enums;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageAccountController(
            IRoleService roleService,
            IAccountService accountService,
            UserManager<ApplicationUser> userManager)
        {
            _roleService = roleService;
            _accountService = accountService;

            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            await PopularRolesAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(AccountSearchViewModel model)
        {
            model.ParseFormData(Request.Form);

            var logs = await _accountService.GetListAsync(
                _userManager.Users,
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
            AccountViewModel model = new AccountCreateViewModel();
            await PopularRolesAndGendersAsync(model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser entity = ParseApplicationUser(model);
                var result = await CreateAsync(entity, model.Password, model.Locked, model.Roles);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                AddErrorsToModelState(result);
            }

            await PopularRolesAndGendersAsync(model);

            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            ApplicationUser entity = await FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var roleNames = await _userManager.GetRolesAsync(entity);
            AccountViewModel model = new AccountUpdateViewModel(entity, roleNames.ToArray());

            await PopularRolesAndGendersAsync(model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AccountUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password) && ModelState.IsValid)
            {
                ApplicationUser entity = await FindAsync(model.Id);

                if (entity == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                entity = ParseApplicationUser(model, entity);

                var result = await UpdateAsync(entity, model.Locked, model.Roles);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                AddErrorsToModelState(result);
            }
            else
            {
                if (model.Password.Length < ValidationRules.MinPasswordLength)
                {
                    ModelState.TryAddModelError("Password", $"The Password must be at least {ValidationRules.MinPasswordLength} characters long.");
                }
                else
                {
                    ApplicationUser entity = await FindAsync(model.Id);

                    if (entity == null)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    var result = await ResetPasswordAsync(entity, model.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    AddErrorsToModelState(result);
                }
            }

            await PopularRolesAndGendersAsync(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int[] ids)
        {
            int? idsCount = ids?.Length;

            if (idsCount == null)
            {
                return BadRequestApiError("Id", "'ApplicationUser Ids' should not be empty.");
            }

            int effectedRows = await DeleteAsync(ids);

            return Ok(effectedRows);
        }

        private async Task PopularRolesAsync()
        {
            var roles = await _roleService.GetListAsync();

            ViewData["Roles"] = roles.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name });
        }

        private async Task PopularRolesAndGendersAsync(AccountViewModel model)
        {
            var roles = await _roleService.GetListAsync();
            model.AllRoles = roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name });

            var genders = (Gender[])Enum.GetValues(typeof(Gender));
            model.AllGenders = genders.Select(g => new SelectListItem { Value = ((byte)g).ToString(), Text = g.ToString() });
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private ApplicationUser ParseApplicationUser(AccountViewModel model, ApplicationUser entity = null)
        {
            bool isCreate = entity == null;
            string email = model.Email.Trim();

            if (isCreate)
            {
                entity = new ApplicationUser
                {
                    EmailConfirmed = true
                };
            }

            entity.UserName = email;
            entity.Email = email;
            entity.FullName = model.FullName.Trim();
            entity.Gender = model.Gender;
            entity.Birthday = model.Birthday;
            entity.PhoneNumber = model.PhoneNumber.TrimNull();
            entity.Address = model.Address.TrimNull();

            return entity;
        }

        #region Functions of ASP.NET Identity Core

        private async Task<ApplicationUser> FindAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            return await _userManager.FindByIdAsync(id.ToString());
        }

        private async Task<IdentityResult> CreateAsync(
            ApplicationUser entity,
            string password,
            bool locked,
            string[] roleNames)
        {
            var result = await _userManager.CreateAsync(entity, password);

            if (result.Succeeded && locked)
            {
                result = await LockAccount(entity);
            }

            if (result.Succeeded && roleNames != null && roleNames.Length != 0)
            {
                result = await _userManager.AddToRolesAsync(entity, roleNames);
            }

            return result;
        }

        private async Task<IdentityResult> UpdateAsync(ApplicationUser entity, bool locked, string[] roleNames)
        {
            var result = await _userManager.UpdateAsync(entity);

            var isLockedOut = await _userManager.IsLockedOutAsync(entity);

            if (locked && !isLockedOut)
            {
                result = await LockAccount(entity);
            }
            else if (!locked && isLockedOut)
            {
                result = await UnlockAccount(entity);
            }

            // Assign Roles.
            var currentRoles = await _userManager.GetRolesAsync(entity);

            if (roleNames == null || roleNames.Length == 0)
            {
                result = await _userManager.RemoveFromRolesAsync(entity, currentRoles);
            }
            else
            {
                var allRoles = await _roleService.GetListAsync();
                var rolesNotExists = roleNames.Except(allRoles.Select(r => r.Name));

                if (rolesNotExists.Any())
                {
                    //throw new ApiException(
                    //    $"Roles '{string.Join(",", rolesNotExists)}' does not exist in the system.",
                    //    ApiErrorCode.NotFound);
                    return IdentityResult.Success;
                }

                result = await _userManager.RemoveFromRolesAsync(entity, currentRoles);

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRolesAsync(entity, roleNames);
                }
            }

            return result;
        }

        private async Task<int> DeleteAsync(int[] ids)
        {
            IdentityResult result;
            int effectedRows = 0;
            ApplicationUser entity;

            foreach (int id in ids)
            {
                entity = await FindAsync(id);

                if (entity != null)
                {
                    result = await _userManager.DeleteAsync(entity);

                    if (result.Succeeded)
                    {
                        effectedRows++;
                    }
                }
            }

            return effectedRows;
        }

        private async Task<IdentityResult> LockAccount(ApplicationUser entity)
        {
            return await _userManager.SetLockoutEndDateAsync(entity, DateTimeOffset.MaxValue);
        }

        private async Task<IdentityResult> UnlockAccount(ApplicationUser entity)
        {
            var result = await _userManager.SetLockoutEndDateAsync(entity, null);

            if (result.Succeeded)
            {
                result = await _userManager.ResetAccessFailedCountAsync(entity);
            }

            return result;
        }

        private async Task<IdentityResult> ResetPasswordAsync(ApplicationUser entity, string password)
        {
            //ApplicationUser entity = await FindAsync(id);

            //if (entity == null)
            //{
            //    throw new ApiException(
            //        $"{nameof(ApplicationUser)} not found. Id = {id}",
            //        ApiErrorCode.NotFound);
            //}

            var result = await _userManager.RemovePasswordAsync(entity);

            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(entity, password);
            }

            return result;
        }

        #endregion
    }
}
