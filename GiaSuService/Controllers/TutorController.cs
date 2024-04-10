using GiaSuService.Configs;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
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
        private readonly ITutorRequestFormService _tutorRequestService;
        private readonly IAuthService _authService;
        private readonly IProfileService _profileService;


        public TutorController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService,
            ITutorRequestFormService tutorRequestService, IAuthService authService, IProfileService profileService)
        {
            _tutorService = tutorService;
            _catalogService = catalogService;
            _addressService = addressService;
            _tutorRequestService = tutorRequestService;
            _authService = authService;
            _profileService = profileService;
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
            gradeList.ForEach(p => p.IsChecked = profile.selectedGradeIds.Contains(p.GradeId));

            var subjectList = await _catalogService.GetAllSubjects();
            subjectList.ForEach(p => p.IsChecked = profile.selectedSubjectIds.Contains(p.SubjectId));

            var sessionList = await _catalogService.GetAllSessions();
            sessionList.ForEach(p => p.IsChecked = profile.selectedSessionIds.Contains(p.SessionId));

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

       
    }
}
