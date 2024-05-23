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
using System.Security.Claims;

namespace GiaSuService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorRequestFormService _tutorRequestService;
        private readonly ITransactionService _transactionService;
        private readonly IProfileService _profileService;

        public HomeController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService, 
            ITutorRequestFormService tutorRequestService, ITransactionService transactionService, IProfileService profileService)
        {
            _tutorService = tutorService;
            _catalogService = catalogService;
            _addressService = addressService;
            _tutorRequestService = tutorRequestService;
            _transactionService = transactionService;
            _profileService = profileService;
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

        private async Task<ProfileViewModel?> GetCurrentCustomer()
        {
            var userRole = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value;

            if (userRole == AppConfig.TUTORROLENAME) return null;

            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            //User not founded
            if (accountId == null || accountId == "" || userRole == null) return null;

            var profileId = await _profileService.GetProfileId(int.Parse(accountId), userRole);

            if (profileId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return null;
            }

            var profile = await _profileService.GetProfile((int)profileId, userRole);
            if (profile == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return null;
            }

            return profile;
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

            var currProfile = await GetCurrentCustomer();
            result.SelectedDistrictId = currProfile == null ? 0 : currProfile.SelectedDistrictId;
            result.SelectedProvinceId = currProfile == null ? 0 : currProfile.SelectedProvinceId;

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorsBy(int provinceId, int districtId, int gradeId, int subjectId, int page)
        {
            var queries = await _tutorService.GetTutorCardsByFilter(provinceId, districtId, subjectId, gradeId, page);
            int totalPages = (int)Math.Ceiling((double)queries.TotalElement / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries=queries.list, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorRequestBy(int provinceId, int districtId, int gradeId, int subjectId, int requestType, int page)
        {
            AppConfig.FormStatus type = AppConfig.FormStatus.APPROVAL;  // requestType == 0
            if(requestType == 1) 
            {
                type = AppConfig.FormStatus.HANDOVER;
            }

            PageTutorRequestListViewModel? queries = await _tutorRequestService.GetTutorrequestCard(provinceId, districtId, gradeId, subjectId, type, page);
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