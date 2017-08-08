using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apex.Admin.Attributes;
using Apex.Admin.ViewModels.DataTables;
using Apex.Admin.ViewModels.Logs;
using Apex.Data.Entities.Logs;
using Apex.Services.Enums;
using Apex.Services.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apex.Admin.Controllers
{
    public class LogController : AdminController
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        //[AdminPermission(Permission.Read)]
        public IActionResult Index()
        {
            PopulateLevels();

            return View();
        }

        [HttpPost]
        //[AdminPermission(Permission.Read)]
        public async Task<IActionResult> Search(LogSearchViewModel model)
        {
            model.ParseFormData(Request.Form);

            var logs = await _logService.GetListAsync(
                model.FromDate,
                model.ToDate,
                model.Levels,
                model.SortColumnName,
                model.SortDirection,
                model.Start,
                model.Length);

            return model.CreateResponse(logs.TotalRecords, logs.TotalRecordsFiltered, logs);
        }

        [HttpPost]
        //[AdminPermission(Permission.Host)]
        public async Task<IActionResult> Delete(int[] ids)
        {
            int effectedRows = 0;
            IEnumerable<Log> entities = await _logService.FindAsync(ids);

            if (entities.Any())
            {
                effectedRows = await _logService.DeleteAsync(entities);
            }

            return Ok(effectedRows);
        }

        private void PopulateLevels()
        {
            IEnumerable<SelectListItem> levels = new List<SelectListItem>
            {
                new SelectListItem { Value = "Fatal", Text = "Fatal" },
                new SelectListItem { Value = "Error", Text = "Error", Selected = true },
                new SelectListItem { Value = "Warn", Text = "Warn" },
                new SelectListItem { Value = "Info", Text = "Info" },
                new SelectListItem { Value = "Debug", Text = "Debug" },
                new SelectListItem { Value = "Trace", Text = "Trace" }
            };

            ViewData["Levels"] = levels;
        }
    }
}
