using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;
using GiaSuService.Repository.Interface;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{
    [Authorize(Policy = AppConfig.ADMINPOLICY)]
    public class AdminController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IAuthService _authService;
        private readonly IProfileService _profileService;
        private readonly ICatalogService _catalogService;
        public AdminController(IAddressService addressService, IAuthService authService, IProfileService profileService, 
            ICatalogService catalogService)
        {
            _addressService = addressService;
            _authService = authService;
            _profileService = profileService;
            _catalogService = catalogService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EmployeeList()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeePage(int page)
        {
            List<AccountListViewModel> accounts = await _profileService.GetEmployeeList(page);
            int totalPages = (int)Math.Ceiling((double)accounts.Count / AppConfig.ROWS_ACCOUNT_LIST);
            var response = new { accounts, page, totalPages };
            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> Register(RegisterAccountProfileViewModel view)
        {
            var provinces = await _addressService.GetProvinces();
         
            RegisterFormViewModel registerFormViewModel = new RegisterFormViewModel()
            {
                ProvinceList = provinces,
            };

            if (view != null)
            {
                registerFormViewModel.RegisterForm = view;
            }

            return View(registerFormViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterFormViewModel model)
        {
            if(!ModelState.IsValid) {
                TempData[AppConfig.MESSAGE_FAIL] = "Lỗi form nhập";
                return RedirectToAction("Register", "Admin", model.RegisterForm);
            }

            var roleId = await _authService.GetRoleId(AppConfig.EMPLOYEEROLENAME);
            if(roleId == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Wrong role here wtf ???"; 
                Console.WriteLine("wtf did i change the name role?");
                return RedirectToAction("Index", "Home");
            }

            var accountProfile = model.RegisterForm!;
            Account account = new Account()
            {
                Email = accountProfile.Email,
                Phone = accountProfile.Phone,
                LockEnable = false,
                Avatar = accountProfile.Avatar,
                RoleId = (int)roleId,
                CreateDate = DateTime.Now,
                PasswordHash = Utility.HashPassword(accountProfile.Password),

                Employee = new Employee()
                {
                    FullName = accountProfile.FullName,
                    Birth = accountProfile.BirthDate,
                    Gender = accountProfile.Gender,
                    AddressDetail = accountProfile.AddressName,
                    DistrictId = accountProfile.SelectedDistrictId,
                    Identity = new IdentityCard()
                    {
                        IdentityNumber = accountProfile.IdentityCard,
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
            }
            return RedirectToAction("", "Admin");
        }

        #region Session Manager
        [HttpGet]
        public IActionResult SessionDateManager()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSessionList()
        {
            var result = await _catalogService.GetAllSessions();
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSession([FromBody] SessionViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.SessionName) || vm.Value == 0)
            {
                return Ok(new ResponseService { Success = false, Message = "Vui lòng điền đủ thông tin" });
            }
            ResponseService mess = await _catalogService.UpdateSessionDate(vm);
            return Ok(mess);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] SessionViewModel vm)
        {
            if(string.IsNullOrEmpty(vm.SessionName) || vm.Value == 0)
            {
                return Ok(new ResponseService { Success = false, Message = "Vui lòng điền đủ thông tin" });
            }
            ResponseService mess = await _catalogService.CreateSessionDate(vm);
            return Ok(mess);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteSession(int id)
        {
            ResponseService mess = await _catalogService.DeleteSessionDate(id);
            return Ok(mess);
        }
        #endregion

        #region Subject Manager
        [HttpGet]
        public IActionResult SubjectManager()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjectList()
        {
            var result = await _catalogService.GetAllSubjects();
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.SubjectName) || vm.Value == 0)
            {
                return Ok(new ResponseService { Success = false, Message = "Vui lòng điền đủ thông tin" });
            }
            ResponseService mess = await _catalogService.UpdateSubject(vm);
            return Ok(mess);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.SubjectName) || vm.Value == 0)
            {
                return Ok(new ResponseService { Success = false, Message = "Vui lòng điền đủ thông tin" });
            }
            ResponseService mess = await _catalogService.CreateSubject(vm);
            return Ok(mess);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            ResponseService mess = await _catalogService.DeleteSubject(id);
            return Ok(mess);
        }
        #endregion

        #region Grade Manager
        public IActionResult GradeManager()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetGradeList()
        {
            var result = await _catalogService.GetAllGrades();
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGrade([FromBody] GradeViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.GradeName) || vm.Value == 0)
            {
                return Ok(new ResponseService { Success = false, Message = "Vui lòng điền đủ thông tin" });
            }
            ResponseService mess = await _catalogService.UpdateGrade(vm);
            return Ok(mess);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromBody] GradeViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.GradeName) || vm.Value == 0)
            {
                return Ok(new ResponseService { Success = false, Message = "Vui lòng điền đủ thông tin" });
            }
            ResponseService mess = await _catalogService.CreateGrade(vm);
            return Ok(mess);
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            ResponseService mess = await _catalogService.DeleteGrade(id);
            return Ok(mess);
        }
        #endregion
    }
}
