using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Data.Entities.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Admin.Controllers
{
    public class ManageAccountController : AdminController
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageAccountController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            PopularRoles();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(AccountSearchViewModel model)
        {
            model.ParseFormData(Request.Form);

            var logs = await _logService.GetListAsync(
                model.FromDate,
                model.ToDate,
                model.Level,
                model.SortColumnName,
                model.SortDirection,
                model.Start,
                model.Length);

            return model.CreateResponse(logs.TotalRecords, logs.TotalRecordsFiltered, logs);
        }

        [NonAction]
        private void PopularRoles()
        {
            IEnumerable<SelectListItem> models = _roleManager.Roles.Select(r => 
                new SelectListItem { Value = r.Id.ToString(), Text = r.Name });

            ViewData["Roles"] = models;
        }
    }
}
