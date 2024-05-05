using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GiaSuService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorRequestFormService _tutorRequestService;

        public HomeController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService, ITutorRequestFormService tutorRequestService)
        {
            _tutorService = tutorService;
            _catalogService = catalogService;
            _addressService = addressService;
            _tutorRequestService = tutorRequestService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = AppConfig.ADMINPOLICY)]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> TutorList()
        {
            var gradeViews = await _catalogService.GetAllGrades();
            var provinceViews = await _addressService.GetProvinces();
            var subjectViews = await _catalogService.GetAllSubjects();

            TutorCardListViewModel result = new TutorCardListViewModel() { 
                GradeList = gradeViews,
                ProvinceList = provinceViews,
                SubjectList = subjectViews,
            };

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorsBy(int districtId, int gradeId, int subjectId, int page)
        {
            var queries = await _tutorService.GetTutorCardsByFilter(subjectId, districtId, gradeId, page);
            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries=queries.list, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorRequestBy(int districtId, int gradeId, int subjectId, int page)
        {
            var queries = await _tutorRequestService.GetTutorrequestCard(districtId, gradeId, subjectId, AppConfig.FormStatus.APPROVAL, page);
            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries=queries.list, page, totalPages };
            return Json(response);
        }
    }
}