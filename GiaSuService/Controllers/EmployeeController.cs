using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{

    public class EmployeeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly IAuthService authService;

        public EmployeeController(ITutorService tutorService, IAuthService authService)
        {
            _tutorService = tutorService;
            this.authService = authService;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> TutorRegisterQueue()
        {
            List<Tutorprofile> queries = await _tutorService.GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus.PENDING);
            IEnumerable<TutorRegisterViewModel> registers = new List<TutorRegisterViewModel>();
            foreach(var query in queries)
            {
                registers.Append(new TutorRegisterViewModel
                {
                    Area = query.Area,
                    College = query.College,
                    CurrentStatus = query.Currentstatus.ToString(),
                    FullName = query.Account.Fullname,
                    Id = query.Id,
                    StatusQuery = query.Formstatus.ToString(),
                });
            }

            return View(registers);
        }
    }
}
