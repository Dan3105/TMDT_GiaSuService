using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace GiaSuService.Controllers
{
    [AllowAnonymous]
    public class IdentityController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAddressService _addressService;
        private readonly ICatalogService _catalogService;

        public IdentityController(IAuthService authService, IAddressService addressService, ICatalogService catalogService)
        {
            _authService = authService;
            _addressService = addressService;
            _catalogService = catalogService;
        }

        public IActionResult Index()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _authService.ValidateAccount(model.Email!, model.Password!);
            if (user != null)
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Fullname),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.Phone),
                    new Claim(ClaimTypes.Role, user.Role.Rolename),
                    new Claim(AppConfig.CLAIM_TYPE_AVATAR, user.Logoaccount)
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, AppConfig.CLAIM_USER);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(AppConfig.AUTHSCHEME, principal);
                //HttpContext.User = principal;

                if (returnUrl.Length > 0)
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("", "Home");
            }

            return View("Index", model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout(string returnUrl = "")
        {
            await HttpContext.SignOutAsync();
            if (returnUrl != null && returnUrl.Length > 0)
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Districts(int provinceId)
        {
            var districts = await _addressService.GetDistricts(provinceId);
            List<DistrictViewModel> list = Utility.ConvertToDistrictViewList(districts);

            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterFormTutor(RegisterAccountProfileViewModel acvm)
        {
            var listSession = await _catalogService.GetAllSessions();
            var listSubject = await _catalogService.GetAllSubjects();
            var listGrade = await _catalogService.GetAllGrades();
            var listProvinces = await _addressService.GetProvinces();
            if (listProvinces == null || listSession == null || listSubject == null || listGrade == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "FCK how tf they get wrong???";
                return RedirectToAction("Index");
            }

            var listSessionView = Utility.ConvertToSessionViewList(listSession);
            var listProvinceView = Utility.ConvertToProvinceViewList(listProvinces);
            var listGradeView = Utility.ConvertToGradeViewList(listGrade);
            var listSubjectView = Utility.ConvertToSubjectViewList(listSubject);

            FormRegisterTutorRequestViewModel form = new FormRegisterTutorRequestViewModel()
            {
                ListProvince = listProvinceView,
                ListSessionDate = listSessionView,
                ListGrade = listGradeView,
                ListSubject = listSubjectView,
            };
            if (acvm != null)
            {
                form.AccountProfile = acvm;
            }


            return View(form);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterFormTutorPost(FormRegisterTutorRequestViewModel model)
        {

            if (!ModelState.IsValid)
            {
                foreach(var valid in ModelState)
                {
                    if(valid.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                    {
                        Console.WriteLine(valid.Value);
                    }
                }

                TempData[AppConfig.MESSAGE_FAIL] = "Thông tin bị thiếu";
                return RedirectToAction("RegisterFormTutor", "Identity", model.AccountProfile);
            }

            int? roleId = await _authService.GetRoleId(AppConfig.TUTORROLENAME);
            if (roleId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "WTF Role";
                return RedirectToAction("", "Home");
            }

            Tutorprofile tutorprofile = new Tutorprofile()
            {
                Academicyearfrom = model.RegisterTutorProfile.AcademicYearFrom,
                Academicyearto = model.RegisterTutorProfile.AcademicYearto,
                Additionalinfo = model.RegisterTutorProfile.AdditionalInfo,
                College = model.RegisterTutorProfile.College,
                Area = model.RegisterTutorProfile.Area,
                Currentstatus = model.RegisterTutorProfile.CurrentStatus,
            };

            Account form = new Account()
            {
                Birth = model.AccountProfile.BirthDate,
                Email = model.AccountProfile.Email,
                Fullname = model.AccountProfile.FullName,
                Gender = model.AccountProfile.Gender,
                Identitycard = model.AccountProfile.IdentityCard,
                Passwordhash = Utility.HashPassword(model.AccountProfile.Password),
                Phone = model.AccountProfile.Phone,
                Districtid = model.AccountProfile.SelectedDistrictId,
                Addressdetail = model.AccountProfile.AddressName,
                Logoaccount = model.AccountProfile.LogoAccount,
                Lockenable = true,
                Roleid = (int)roleId,
                Tutorprofile = tutorprofile
            };
            var listGrade = model.GetGradeSelected.Select(p => p.GradeId);
            var listSession = model.GetSessionSelected.Select(p => p.SessionId);
            var listSubject = model.GetSubjectSelected.Select(p => p.SubjectId);
            tutorprofile.Account = form;
            bool isSuccess = await _authService.CreateTutorRegisterRequest(form, tutorprofile, model.ListDistrict, listGrade, listSubject, listSession);
            if (isSuccess)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Form has been applied";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Error in request";
                return RedirectToAction("RegisterFormTutor", "Identity", model.AccountProfile);
            }
        }


    }
}
