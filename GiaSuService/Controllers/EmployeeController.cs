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
        private readonly IAuthService _authService;

        public EmployeeController(ITutorService tutorService, IAuthService authService)
        {
            _tutorService = tutorService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> TutorRegisterQueue()
        {
            List<Tutorprofile> queries = await _tutorService.GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus.PENDING);
            List<TutorRegisterViewModel> registers = new List<TutorRegisterViewModel>();
            foreach(var query in queries)
            {
                registers.Add(new TutorRegisterViewModel()
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
