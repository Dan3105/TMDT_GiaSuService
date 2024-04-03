using GiaSuService.Configs;
using GiaSuService.Models.IdentityViewModel;
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

            var profile = await _profileService.GetTutorProfile((int)profileId);

            if (profile == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }
            
            return View(profile);
        }

        [Authorize(Policy = AppConfig.TUTORPOLICY)]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(TutorProfileViewModel profile)
        {
            ResponseService response = await _profileService.UpdateTutorProfile(profile);
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
        public IActionResult TutorRequestList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTutorRequestBy(int page)
        {
            var queries = await _tutorRequestService.GetTutorrequestCard(AppConfig.FormStatus.APPROVAL, page);
            int totalPages = (int)Math.Ceiling((double)queries.Count / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { queries, page, totalPages };
            return Json(response);
        }
    }
}
