using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.TutorViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace GiaSuService.Controllers
{
    [AllowAnonymous]
    public class IdentityController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAddressService _addressService;
        private readonly ICatalogService _catalogService;
        private readonly IProfileService _profileService;

        public IdentityController(IAuthService authService, IAddressService addressService, ICatalogService catalogService, IProfileService profileService)
        {
            _authService = authService;
            _addressService = addressService;
            _catalogService = catalogService;
            _profileService = profileService;
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

            var user = await _authService.ValidateAccount(model.LoginName!, model.Password!);
            if (user != null && !user.LockEnable)
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.Phone),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim(AppConfig.CLAIM_TYPE_AVATAR, user.Avatar)
                };

                ClaimsIdentity identity = new ClaimsIdentity(claims, AppConfig.CLAIM_USER);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(AppConfig.AUTHSCHEME, principal);

                TempData[AppConfig.MESSAGE_SUCCESS] = $"Hello {user.Email}";
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
        [Authorize(Policy = AppConfig.PROFILE_POLICY)]
        public async Task<IActionResult> Profile()
        {
            var userRole = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value;

            if (userRole == AppConfig.TUTORROLENAME) return RedirectToAction("Profile", "Tutor");

            var accountId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            //User not founded
            if (accountId == null || accountId == "" || userRole == null) return RedirectToAction("Index", "Home");

            var profileId = await _profileService.GetProfileId(int.Parse(accountId), userRole);

            if (profileId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            var profile = await _profileService.GetProfile((int)profileId, userRole);
            if (profile == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Mã tài khoản không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            return View(profile);
        }

        [HttpPost]
        [Authorize(Policy = AppConfig.PROFILE_POLICY)]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel profile)
        {
            var userRole = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Role)?.Value;
            if (userRole == null) return RedirectToAction("Index", "Home");

            ResponseService response = await _profileService.UpdateProfile(profile, userRole);
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
        public async Task<IActionResult> Provinces()
        {
            var provinces = await _addressService.GetProvinces();
            return Json(provinces);
        }

        [HttpGet]
        public async Task<IActionResult> Districts(int provinceId)
        {
            var districts = await _addressService.GetDistricts(provinceId);
            return Json(districts);
        }

        [HttpGet]
        public async Task<IActionResult> RegisterFormTutor(RegisterAccountProfileViewModel acvm)
        {
            var listSession = await _catalogService.GetAllSessions();
            var listSubject = await _catalogService.GetAllSubjects();
            var listGrade = await _catalogService.GetAllGrades();
            var listProvinces = await _addressService.GetProvinces();
            var listTypeTutor = await _catalogService.GetAllTutorType();
            if (listProvinces == null || listSession == null || listSubject == null || listGrade == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Hệ thống cập nhật thiếu dữ liệu vui lòng làm lại";
                return RedirectToAction("Index");
            }

            FormRegisterTutorRequestViewModel form = new FormRegisterTutorRequestViewModel()
            {
                ListProvince = listProvinces,
                ListSessionDate = listSession,
                ListGrade = listGrade,
                ListSubject = listSubject,
                ListTutorType = listTypeTutor
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
                        //Console.WriteLine(valid.Value);
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

            if (model.RegisterTutorProfile.AcademicYearto <= model.RegisterTutorProfile.AcademicYearFrom)
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

            var listGrade = model.GetGradeSelected.Select(p => p.GradeId);
            var listSession = model.GetSessionSelected.Select(p => p.SessionId);
            var listSubject = model.GetSubjectSelected.Select(p => p.SubjectId);
     
            Account account = new Account()
            {
                Email = model.AccountProfile.Email,
                Phone = model.AccountProfile.Phone,
                PasswordHash = Utility.HashPassword(model.AccountProfile.Password),
                RoleId = (int)roleId,
                LockEnable = false,
                Avatar = model.AccountProfile.Avatar,
                CreateDate = DateTime.Now,
                Tutor = new Tutor()
                {
                    Birth = model.AccountProfile.BirthDate,
                    FullName = model.AccountProfile.FullName,
                    AddressDetail = model.AccountProfile.AddressName,
                    DistrictId = model.AccountProfile.SelectedDistrictId,
                    Gender = model.AccountProfile.Gender,
                     
                    //Hoc van
                    AcademicYearFrom = model.RegisterTutorProfile.AcademicYearFrom,
                    AcademicYearTo = model.RegisterTutorProfile.AcademicYearto,
                    AdditionalInfo = model.RegisterTutorProfile.AdditionalInfo,
                    College = model.RegisterTutorProfile.College,
                    Area = model.RegisterTutorProfile.Area,
                    TutorTypeId = model.RegisterTutorProfile.TypeTutorId,
                
                    IsActive = true,
                    //Isvalid = false,

                    Identity = new IdentityCard()
                    {
                        IdentityNumber = model.AccountProfile.IdentityCard,
                        FrontIdentityCard = model.AccountProfile.FrontIdentityCard,
                        BackIdentityCard = model.AccountProfile.BackIdentityCard,
                    }, 
                }
                
            };

            var result = await _authService.CreateTutorRegisterRequest(account, listSession, listSubject, listGrade, 
                model.ListDistrict);
            if (result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = result.Message;
                return RedirectToAction("RegisterFormTutor", "Identity", model.AccountProfile);
            }
        }

        [HttpGet]
        public async Task<IActionResult> RegisterFormCustomer(RegisterAccountProfileViewModel view) 
        {
            var provinces = await _addressService.GetProvinces();
           
            RegisterFormViewModel registerFormViewModel = new RegisterFormViewModel()
            {
                ProvinceList = provinces,
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
                Email = accountProfile.Email,
                Phone = accountProfile.Phone,
                PasswordHash = Utility.HashPassword(accountProfile.Password),
                Avatar = accountProfile.Avatar,
                CreateDate = DateTime.Now,
                LockEnable = false,
                RoleId = (int)roleId,
                
                Customer = new Customer()
                {
                    FullName = accountProfile.FullName,
                    Birth = accountProfile.BirthDate,
                    Gender = accountProfile.Gender,
                    AddressDetail = accountProfile.AddressName,
                    DistrictId = accountProfile.SelectedDistrictId,

                    Identity = new IdentityCard()
                    {
                        IdentityNumber= accountProfile.IdentityCard,
                        FrontIdentityCard = accountProfile.FrontIdentityCard,
                        BackIdentityCard = accountProfile.BackIdentityCard,
                    }
                }
            };
            ResponseService result = await _authService.CreateAccount(account);
            if (result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = result.Message;
                return RedirectToAction("RegisterFormCustomer", "Identity", form.RegisterForm);
            }
            return RedirectToAction("", "Identity");
        }
    }
}
