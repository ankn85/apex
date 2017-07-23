using System.Collections.Generic;
using System.Threading.Tasks;
using Apex.Admin.ViewModels.DataTables;
using Apex.Admin.ViewModels.Logs;
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

        public IActionResult Index()
        {
            PopularLogLevels();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(LogSearchViewModel model)
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

        [HttpPost]
        public async Task<IActionResult> Delete(int[] ids)
        {
            if (ids != null && ids.Length > 0)
            {
                await _logService.DeleteAsync(ids);

                return Ok();
            }

            return BadRequestApiError("LogId", "'Log Ids' should not be empty.");
        }

        [NonAction]
        private void PopularLogLevels()
        {
            IEnumerable<SelectListItem> models = new List<SelectListItem>
            {
                new SelectListItem { Value = string.Empty, Text = "All", Selected = true },
                new SelectListItem { Value = "Fatal", Text = "Fatal" },
                new SelectListItem { Value = "Error", Text = "Error" },
                new SelectListItem { Value = "Warn", Text = "Warn" },
                new SelectListItem { Value = "Info", Text = "Info" },
                new SelectListItem { Value = "Debug", Text = "Debug" },
                new SelectListItem { Value = "Trace", Text = "Trace" }
            };

            ViewData["LogLevels"] = models;
        }
    }
}
