using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Services;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GiaSuService.Controllers
{
    
    public class TutorController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly IProfileService _profileService;
        private readonly ITransactionService _transactionService;


        public TutorController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService,
             IProfileService profileService, ITransactionService transactionService)
        {
            _tutorService = tutorService;
            _catalogService = catalogService;
            _addressService = addressService;
            _profileService = profileService;
            _transactionService = transactionService;
        }

        public IActionResult Index()
        {
            return Ok();
        }

        [Authorize(Policy = AppConfig.TUTORPOLICY)]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null || accountId == "" ) return RedirectToAction("Index", "Home");

            int? profileId = await _profileService.GetProfileId(int.Parse(accountId), AppConfig.TUTORROLENAME);

            if (profileId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            var profile = await _profileService.GetTutorFormUpdateById((int)profileId);

            if (profile == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            var provinceList = await _addressService.GetProvinces();

            var gradeList = await _catalogService.GetAllGrades();
            gradeList.ForEach(p => p.IsChecked = profile.SelectedGradeIds.Contains(p.GradeId));

            var subjectList = await _catalogService.GetAllSubjects();
            subjectList.ForEach(p => p.IsChecked = profile.SelectedSubjectIds.Contains(p.SubjectId));

            var sessionList = await _catalogService.GetAllSessions();
            sessionList.ForEach(p => p.IsChecked = profile.SelectedSessionIds.Contains(p.SessionId));

            var tutorTypeList = await _catalogService.GetAllTutorType();

            TutorUpdateRequestViewModel model = new TutorUpdateRequestViewModel()
            {
                Form = profile,
                ProvinceList = provinceList,
                GradeList = gradeList,
                SubjectList = subjectList,
                SessionList = sessionList,
                TutorTypeList = tutorTypeList
            };

            return View(model);
        }

        [Authorize(Policy = AppConfig.TUTORPOLICY)]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(TutorUpdateRequestViewModel profile)
        {

            ResponseService response = await _profileService.CreateRequestTutorProfile(profile);
            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = response.Message;
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }
            return RedirectToAction("Profile", "Identity");
        }

        [HttpGet]
        public async Task<IActionResult> TutorRequestList()
        {
            var gradeViews = await _catalogService.GetAllGrades();
            var provinceViews = await _addressService.GetProvinces();
            var subjectViews = await _catalogService.GetAllSubjects();

            TutorRequestListViewModel result = new TutorRequestListViewModel()
            {
                GradeList = gradeViews,
                ProvinceList = provinceViews,
                SubjectList = subjectViews,
            };

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> TutorProfileStatusHistory()
        {
            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null || accountId == "") return RedirectToAction("Index", "Home");

            int? tutorId = await _profileService.GetProfileId(int.Parse(accountId), AppConfig.TUTORROLENAME);

            if (tutorId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            var histories = await _tutorService.GetStatusTutorHistory((int)tutorId);

            return View(histories);
        }

        [HttpGet]
        public async Task<IActionResult> TutorProfileStatusDetail(int historyId)
        {
            var data = await _tutorService.GetAStatusTutorHistory(historyId);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> ApplyRequest(int requestId)
        {
            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null || accountId == "") return RedirectToAction("Index", "Home");

            int? tutorId = await _profileService.GetProfileId(int.Parse(accountId), AppConfig.TUTORROLENAME);

            if (tutorId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }
            ResponseService response = await _tutorService.ApplyRequest((int)tutorId, requestId);
            if (response.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = response.Message;
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = response.Message;
            }

            return RedirectToAction("TutorRequestList", "Tutor");
        }


        [HttpGet]
        public async Task<IActionResult> ListTutorApplyForm()
        {
            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null || accountId == "") return RedirectToAction("Index", "Home");

            int? tutorId = await _profileService.GetProfileId(int.Parse(accountId), AppConfig.TUTORROLENAME);

            if (tutorId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            var listApplyForm = await _tutorService.GetTutorApplyForm((int)tutorId);
            return View(listApplyForm);
        }

        [HttpGet]
        public async Task<IActionResult> TutorTransactionList()
        {
            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;

            if (accountId == null || accountId == "") return RedirectToAction("Index", "Home");

            int? tutorId = await _profileService.GetProfileId(int.Parse(accountId), AppConfig.TUTORROLENAME);

            if (tutorId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            var results = await _transactionService.GetListTutorTransaction((int)tutorId);
            return View(results);   
        }
    }
}
