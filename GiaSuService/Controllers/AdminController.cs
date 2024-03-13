using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.AdminViewModel;
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
        public async Task<IActionResult> Register(RegisterFormViewModel view)
        {
            var provinces = await _addressService.GetProvinces();
            RegisterEmployeeViewModel registerFormViewModel = new RegisterEmployeeViewModel()
            { 
                ProvinceList = provinces 
            };

            if(view != null)
            {
                registerFormViewModel.RegisterForm = view;
            }
           
            return View(registerFormViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterEmployeeViewModel model)
        {
            if(!ModelState.IsValid) {
                return RedirectToAction("Register", "Admin", model.RegisterForm);
            }

            var roleId = await _authService.GetRoleId(AppConfig.EMPLOYEEROLENAME);
            if(roleId == null)
            {
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
                Gender = accountProfile.Gender,
                Lockenable = false,
                Logoaccount = accountProfile.LogoAccount,
                Roleid = (int)roleId,
                Districtid = accountProfile.AddressVM!.District!.DistrictId,
                Addressdetail = accountProfile.AddressVM!.AddressName,

                
            };
            await _authService.CreateAccount(account);
            return RedirectToAction("", "Admin");
        }

    }
}
