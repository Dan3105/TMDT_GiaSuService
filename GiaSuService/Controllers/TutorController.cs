using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.EmployeeViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{
    public class TutorController : Controller
    {
        private readonly ITutorService _tutorService;
        private readonly ICatalogService _catalogService;
        private readonly IAddressService _addressService;
        private readonly ITutorRequestFormService _tutorRequestService;
        private readonly IAuthService _authService;


        //public TutorController(ITutorService tutorService, ICatalogService catalogService, IAddressService addressService,
        //    ITutorRequestFormService tutorRequestService, IAuthService authService)
        //{
        //    _tutorService = tutorService;
        //    _catalogService = catalogService;
        //    _addressService = addressService;
        //    _tutorRequestService = tutorRequestService;
        //    _authService = authService;
        //}

        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        [Authorize(Policy = AppConfig.TUTORPOLICY)]
        public async Task<IActionResult> TutorProfile(int id)
        {
            //Tutor tutor = await _tutorService.GetTutorprofileByAccountId(id);
            Tutor tutor = null!;
            if (tutor == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "User cannot be found";
                return RedirectToAction("Index", "Employee");
            }
            //District district = await _addressService.GetDistrictData(tutor.Account.Districtid);
            //District district = await _addressService.GetDistrictData(1);
            //TutorProfileInEmployeeViewModel view = new TutorProfileInEmployeeViewModel();
            //{
            //    Academicyearfrom = tutor.Academicyearfrom,
            //    Academicyearto = tutor.Academicyearto,
            //    Additionalinfo = tutor.Additionalinfo,
            //    Address = $"{district.Province.Provincename}, {district.Districtname}, {tutor.Account.Addressdetail}",
            //    Area = tutor.Area,
            //    Avatar = tutor.Account.Avatar,
            //    Backidentitycard = tutor.Account.Backidentitycard,
            //    Birth = tutor.Account.Birth,
            //    College = tutor.College,
            //    Createdate = tutor.Account.Createdate,
            //    Currentstatus = tutor.Currentstatus.ToString(),
            //    Email = tutor.Account.Email,
            //    Formstatus = tutor.Formstatus,
            //    Frontidentitycard = tutor.Account.Frontidentitycard,
            //    Fullname = tutor.Account.Fullname,
            //    Gender = tutor.Account.Gender,
            //    Identitycard = tutor.Account.Identitycard,
            //    Lockenable = tutor.Account.Lockenable,
            //    Phone = tutor.Account.Phone,
            //    TutorId = tutor.Id,
            //};
            return View();
        }
    }
}
