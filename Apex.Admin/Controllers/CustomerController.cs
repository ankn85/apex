﻿using System;
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
    public class CustomerController : AdminController
    {
        private readonly IRoleService _roleService;
        private readonly IAccountService _accountService;

        public CustomerController(
            IRoleService roleService,
            IAccountService accountService)
        {
            _roleService = roleService;
            _accountService = accountService;
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
            await AssignRolesAndGendersAsync(model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountCreateViewModel model)
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

            await AssignRolesAndGendersAsync(model);

            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            ApplicationUser entity = await _accountService.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var roleNames = await _accountService.GetRolesAsync(entity);
            AccountViewModel model = new AccountUpdateViewModel(entity, roleNames.ToArray());

            await AssignRolesAndGendersAsync(model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AccountUpdateViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser entity = await _accountService.FindAsync(model.Id);

                    if (entity == null)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    entity = ParseApplicationUser(model, entity);

                    var result = await _accountService.UpdateAsync(entity, model.Locked, model.Roles);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    AddErrorsToModelState(result);
                }
            }
            else
            {
                if (model.Password.Length < ValidationRules.MinPasswordLength)
                {
                    ModelState.TryAddModelError("Password", $"The Password must be at least {ValidationRules.MinPasswordLength} characters long.");
                }
                else
                {
                    ApplicationUser entity = await _accountService.FindAsync(model.Id);

                    if (entity == null)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    var result = await _accountService.ResetPasswordAsync(entity, model.Password);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    AddErrorsToModelState(result);
                }
            }

            await AssignRolesAndGendersAsync(model);

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

            int effectedRows = await _accountService.DeleteAsync(ids);

            return Ok(effectedRows);
        }

        private async Task PopularRolesAsync()
        {
            var roles = await _roleService.GetListAsync();

            ViewData["Roles"] = roles.Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name });
        }

        private async Task AssignRolesAndGendersAsync(AccountViewModel model)
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
    }
}
