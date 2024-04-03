﻿using GiaSuService.Configs;
using GiaSuService.EntityModel;
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
        private readonly IProfileService _profileService;
        public AdminController(IAddressService addressService, IAuthService authService, IProfileService profileService)
        {
            _addressService = addressService;
            _authService = authService;
            _profileService = profileService;
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
        public async Task<IActionResult> EmployeeProfile(int id)
        {
            var employee = await _profileService.GetEmployeeProfile(id);
            if(employee == null)
            {
                TempData[AppConfig.MESSAGE_FAIL] = "Tdn mã nhân viên không tồn tại";
                return RedirectToAction("EmployeeList", "Admin");
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeProfile(ProfileViewModel employeeProfileViewModel)
        {
            ResponseService result = await _profileService.UpdateEmployeeProfile(employeeProfileViewModel);
            if (result.Success)
            {
                TempData[AppConfig.MESSAGE_SUCCESS] = result.Message;
                return RedirectToAction("EmployeeList", "Admin");
            }
            else
            {
                TempData[AppConfig.MESSAGE_FAIL] = result.Message;
                return RedirectToAction("EmployeeList", "Admin");
            }
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
                Lockenable = false,
                Avatar = accountProfile.Avatar,
                Roleid = (int)roleId,
                Createdate = DateTime.Now,
                Passwordhash = Utility.HashPassword(accountProfile.Password),

                Employee = new Employee()
                {
                    Fullname = accountProfile.FullName,
                    Birth = accountProfile.BirthDate,
                    Gender = accountProfile.Gender,
                    Addressdetail = accountProfile.AddressName,
                    Districtid = accountProfile.SelectedDistrictId,
                    Identity = new Identitycard()
                    {
                        Identitynumber = accountProfile.IdentityCard,
                        Frontidentitycard = accountProfile.FrontIdentityCard,
                        Backidentitycard = accountProfile.BackIdentityCard,
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

    }
}
