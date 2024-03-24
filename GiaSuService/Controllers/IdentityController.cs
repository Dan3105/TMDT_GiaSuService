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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GiaSuService.Controllers
{
    [AllowAnonymous]
    public class IdentityController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAddressService _addressService;
        private readonly ICatalogService _catalogService;
        private readonly ITutorService _tutorService;

        public IdentityController(IAuthService authService, IAddressService addressService, ICatalogService catalogService, ITutorService tutorService)
        {
            _authService = authService;
            _addressService = addressService;
            _catalogService = catalogService;
            _tutorService = tutorService;
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
            if (user != null && !user.Lockenable)
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Fullname),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.Phone),
                    new Claim(ClaimTypes.Role, user.Role.Rolename),
                    new Claim(AppConfig.CLAIM_TYPE_AVATAR, user.Avatar)
                };

                if(user.Role.Rolename == AppConfig.EMPLOYEEROLENAME)
                {
                    int count_querying_register = (await _tutorService.GetTutorprofilesByRegisterStatus(AppConfig.RegisterStatus.PENDING)).Count();
                    TempData["Count"] = count_querying_register.ToString();
                }

                ClaimsIdentity identity = new ClaimsIdentity(claims, AppConfig.CLAIM_USER);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(AppConfig.AUTHSCHEME, principal);
                //HttpContext.User = principal;

                TempData[AppConfig.MESSAGE_SUCCESS] = $"Hello {user.Fullname}";
                if (returnUrl.Length > 0)
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("", "Home");
            }
            TempData[AppConfig.MESSAGE_FAIL] = "Incorrect Login";
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

            if (model.ListDistrict.Count == 0 || model.ListSessionDate.Count == 0 || model.ListGrade.Count == 0 || model.ListSubject.Count==0)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Thông tin lựa chọn bị thiếu";
                return RedirectToAction("RegisterFormTutor", "Identity", model.AccountProfile);
            }

            if (model.RegisterTutorProfile.AcademicYearto >= model.RegisterTutorProfile.AcademicYearFrom)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Thông tin chọn năm tốt nghiệp không chính xác";
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
                Currentstatus = model.RegisterTutorProfile.TypeTutor,
            };

            Account form = new Account()
            {
                Birth = model.AccountProfile.BirthDate,
                Email = model.AccountProfile.Email,
                Fullname = model.AccountProfile.FullName,
                Gender = model.AccountProfile.Gender,
                Identitycard = model.AccountProfile.IdentityCard,
                Passwordhash = Utility.HashPassword(model.AccountProfile.Password),
                Frontidentitycard = model.AccountProfile.FrontIdentityCard,
                Backidentitycard = model.AccountProfile.BackIdentityCard,
                Phone = model.AccountProfile.Phone,
                Districtid = model.AccountProfile.SelectedDistrictId,
                Addressdetail = model.AccountProfile.AddressName,
                Avatar = model.AccountProfile.LogoAccount,
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

        [HttpGet]
        public async Task<IActionResult> RegisterFormCustomer(RegisterAccountProfileViewModel view) 
        {
            var provinces = await _addressService.GetProvinces();
            List<ProvinceViewModel> result = Utility.ConvertToProvinceViewList(provinces);

            RegisterFormViewModel registerFormViewModel = new RegisterFormViewModel()
            {
                ProvinceList = result,
                RegisterForm = new RegisterAccountProfileViewModel() { }
            };

            if (view != null)
            {
                registerFormViewModel.RegisterForm = view;
            }
            return View(registerFormViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterFormCustomer(RegisterFormViewModel form)
        {
            if (!ModelState.IsValid)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Lỗi form nhập";
                return RedirectToAction("RegisterFormCustomer", "Identity", form.RegisterForm);
            }

            var roleId = await _authService.GetRoleId(AppConfig.CUSTOMERROLENAME);
            if (roleId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Wrong role here wtf ???";
                Console.WriteLine("wtf did i change the name role?");
                return RedirectToAction("Index", "Home");
            }

            var accountProfile = form.RegisterForm!;
            Account account = new Account()
            {
                Fullname = accountProfile.FullName,
                Birth = accountProfile.BirthDate,
                Email = accountProfile.Email,
                Phone = accountProfile.Phone,
                Passwordhash = Utility.HashPassword(accountProfile.Password),
                Identitycard = accountProfile.IdentityCard,
                Frontidentitycard = accountProfile.FrontIdentityCard,
                Backidentitycard = accountProfile.BackIdentityCard,
                Gender = accountProfile.Gender,
                Lockenable = false,
                Avatar = accountProfile.LogoAccount,
                Roleid = (int)roleId,
                Addressdetail = accountProfile.AddressName,
                Districtid = accountProfile.SelectedDistrictId,
                Createdate = DateOnly.FromDateTime(DateTime.Now)
            };
            bool isSuccess = await _authService.CreateAccount(account);
            if (isSuccess)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Tạo tài khoản thành công";
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Tạo tài khoản thất bại";
            }
            return RedirectToAction("", "Home");
        }
    }
}
