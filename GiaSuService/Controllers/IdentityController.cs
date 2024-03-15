using GiaSuService.Configs;
using GiaSuService.EntityModel;
using GiaSuService.Models.IdentityViewModel;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GiaSuService.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAddressService _addressService;
        public IdentityController(IAuthService authService, IAddressService addressService)
        {
            _authService = authService;
            _addressService = addressService;
        }

        public IActionResult Index()
        {
            if(HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if(!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var user = await _authService.ValidateAccount(model.Email!, model.Password!);
            if(user != null)
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

                if(returnUrl.Length > 0)
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
            if(returnUrl != null && returnUrl.Length > 0)
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Districts(int provinceId)
        {
            var districts = await _addressService.GetDistricts(provinceId);
            List<DistrictViewModel> result = new List<DistrictViewModel>();
            foreach (District district in districts)
            {
                result.Add(new DistrictViewModel
                {
                    DistrictId = district.Id,
                    DistrictName = district.Districtname,
                    ProvinceId = district.Provinceid,
                });
            }
            return Ok(result);
        }
    }
}
