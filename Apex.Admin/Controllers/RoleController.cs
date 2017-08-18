using System.Threading.Tasks;
using Apex.Admin.ViewModels.Accounts;
using Apex.Admin.ViewModels.DataTables;
using Apex.Data.Entities.Accounts;
using Apex.Services.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    public class RoleController : AdminController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(DataTablesRequest model)
        {
            model.ParseFormData(Request.Form);

            var roles = await _roleService.GetListAsync(model.SortColumnName, model.SortDirection);

            return model.CreateResponse(roles.TotalRecords, roles.TotalRecordsFiltered, roles);
        }

        public IActionResult Create()
        {
            return View("Save", new RoleViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole entity = ParseApplicationRole(model);

                var existedRole = await _roleService.RoleExistsAsync(model.RoleName);

                if (!existedRole)
                {
                    var result = await _roleService.CreateAsync(entity);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    AddErrorsToModelState(result);
                }
                else
                {
                    ModelState.AddModelError(nameof(RoleViewModel.RoleName), "The Role Name field is duplicated.");
                }
            }

            return View("Save", model);
        }

        public async Task<IActionResult> Update(int id)
        {
            ApplicationRole entity = await _roleService.FindAsync(id);

            if (entity == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("Save", new RoleViewModel(entity));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole entity = await _roleService.FindAsync(model.Id);

                if (entity == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                entity = ParseApplicationRole(model, entity);

                var existedRole = await _roleService.RoleExistsAsync(model.RoleName);

                if (!existedRole)
                {
                    var result = await _roleService.UpdateAsync(entity);

                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    AddErrorsToModelState(result);
                }
                else
                {
                    ModelState.AddModelError(nameof(RoleViewModel.RoleName), "The Role Name field is duplicated.");
                }
            }

            return View("Save", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int[] ids)
        {
            int effectedRows = 0;

            if (ids != null && ids.Length > 0)
            {
                effectedRows = await _roleService.DeleteAsync(ids);
            }

            return Ok(effectedRows);
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private ApplicationRole ParseApplicationRole(RoleViewModel model, ApplicationRole entity = null)
        {
            entity = entity ?? new ApplicationRole();
            entity.Name = model.RoleName.Trim();

            return entity;
        }
    }
}
