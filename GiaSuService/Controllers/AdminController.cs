using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.AdminViewModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Models.UtilityViewModel;
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
        public AdminController(IAddressService addressService, IAuthService authService)
        {
            _addressService = addressService;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeList()
        {
            var accounts = await _authService.GetAccountsByRole(AppConfig.EMPLOYEEROLENAME);
            if(accounts == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Wrong role here wtf ???";
                Console.WriteLine("wtf did i change the name role?");
                return RedirectToAction("Index", "Home");
            }

            List<EmployeeListViewModel> results = new List<EmployeeListViewModel>();
            foreach(var account in accounts)
            {
                results.Add(new EmployeeListViewModel()
                {
                    Id = account.Id,
                    Email = account.Email,
                    FullName = account.Fullname,
                    LockStatus = account.Lockenable,
                    ImageUrl = account.Avatar
                });

            }

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeProfile(int id)
        {
            Account account = await _authService.GetAccountById(id);
            if(account == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Tdn mã nhân viên không tồn tại";
                return RedirectToAction("EmployeeList", "Admin");
            }

            District district = await _addressService.GetDistrictData(account.Districtid);
            EmployeeProfileViewModel employeeProfileViewModel = new EmployeeProfileViewModel()
            {
                LogoAccount = account.Avatar,
                Phone = account.Phone,
                IdentityCard = account.Identitycard,
                FrontIdentiyCard = account.Frontidentitycard,
                BackIdentityCard = account.Backidentitycard,
                Gender = account.Gender,
                Email = account.Email,
                AddressDetail = district.Province.Provincename + " " + district.Districtname + " " + account.Addressdetail,
                FullName = account.Fullname,
                LockStatus = account.Lockenable,
                BirthDate = account.Birth,
                EmployeeId = account.Id
            };
            return View(employeeProfileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeProfile(EmployeeProfileViewModel employeeProfileViewModel)
        {
            Account account = await _authService.GetAccountById(employeeProfileViewModel.EmployeeId);
            if (account == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Tdn mã nhân viên không tồn tại";
                return RedirectToAction("EmployeeList", "Admin");
            }
            account.Identitycard = employeeProfileViewModel.IdentityCard;
            account.Lockenable = employeeProfileViewModel.LockStatus;

            bool result = await _authService.UpdateAccount(account);
            if (result)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = "Thay đổi thông tin thành công";
                return RedirectToAction("EmployeeList", "Admin");
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Thay đổi không thông tin thành công";
                return RedirectToAction("EmployeeList", "Admin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register(RegisterAccountProfileViewModel view)
        {
            var provinces = await _addressService.GetProvinces();
            List<ProvinceViewModel> result = Utility.ConvertToProvinceViewList(provinces);

            RegisterFormViewModel registerFormViewModel = new RegisterFormViewModel()
            {
                ProvinceList = result,
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
                TempData[AppConfig.MESSAGE_SUCCESS] = "Adding user success";
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Some fking error happend idk";
            }
            return RedirectToAction("", "Admin");
        }

    }
}
