using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace GiaSuService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorRequestFormService _tutorRequestService;
        private readonly ITransactionService _transactionService;

        public HomeController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService, 
            ITutorRequestFormService tutorRequestService, ITransactionService transactionService)
        {
            _tutorService = tutorService;
            _catalogService = catalogService;
            _addressService = addressService;
            _tutorRequestService = tutorRequestService;
            _transactionService = transactionService;
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
        public async Task<IActionResult> GetTutorRequestBy(int districtId, int gradeId, int subjectId, int requestType, int page)
        {
            PageTutorRequestListViewModel? queries = null;
            if(requestType == 1) 
            { 
                queries = await _tutorRequestService.GetTutorrequestCard(districtId, gradeId, subjectId, AppConfig.FormStatus.HANDOVER, page); 
            }
            else
            {
                queries = await _tutorRequestService.GetTutorrequestCard(districtId, gradeId, subjectId, AppConfig.FormStatus.APPROVAL, page);
            }

            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries=queries.list, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorRequestById(int requestId)
        {
            TutorRequestCardViewModel? queries = await _tutorRequestService.GetTutorrequestDetail(requestId);
            if(queries != null && 
            queries.RequestStatus != AppConfig.FormStatus.APPROVAL.ToString().ToLower() 
            && queries.RequestStatus != AppConfig.FormStatus.HANDOVER.ToString().ToLower())
            {
                queries = null;
            }

            int page = 1;
            int totalPages = (int)Math.Ceiling(1.0 / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries = queries, page, totalPages };
            return Json(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetSubjects()
        {
            var obj = await _catalogService.GetAllSubjects();
            return Json(obj);
        }

        [HttpGet]
        public async Task<IActionResult> GetListTransaction(int payStatus = 0, int transactionType = 0, int page = 0)
        {
            AppConfig.TransactionFilterStatus status = (payStatus == 1 ? AppConfig.TransactionFilterStatus.PAID :
                                                        (payStatus == 2 ? AppConfig.TransactionFilterStatus.UNPAID : AppConfig.TransactionFilterStatus.ALL));

            AppConfig.TransactionFilterType type = (transactionType == 1 ? AppConfig.TransactionFilterType.PAID :
                                                    (transactionType == 2 ? AppConfig.TransactionFilterType.REFUND : AppConfig.TransactionFilterType.ALL));


            var queries = await _transactionService.GetListTransaction(status, type, page);
            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / (double)AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries = queries.list, page, totalPages };
            return Json(response);
        }
    }
}