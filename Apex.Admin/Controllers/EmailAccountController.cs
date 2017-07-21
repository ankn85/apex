using System.Threading.Tasks;
using Apex.Admin.ViewModels.DataTables;
using Apex.Services.Emails;
using Microsoft.AspNetCore.Mvc;

namespace Apex.Admin.Controllers
{
    public class EmailAccountController : AdminController
    {
        private readonly IEmailAccountService _emailAccountService;

        public EmailAccountController(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(DataTablesRequest model)
        {
            model.ParseFormData(Request.Form);

            var emailAccounts = await _emailAccountService.GetListAsync(model.SortColumnName, model.SortDirection);

            return model.CreateResponse(emailAccounts.TotalRecords, emailAccounts.TotalRecordsFiltered, emailAccounts);
        }
    }
}
