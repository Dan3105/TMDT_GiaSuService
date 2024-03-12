using GiaSuService.Configs;
using GiaSuService.Models;
using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace GiaSuService.Controllers
{
    public class IdentityController : Controller
    {
        private readonly IAuthService _authService;
        public IdentityController(IAuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "")
        {
            if(!ModelState.IsValid)
            {
                /*foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // Log or display errors to the user
                    }
                }*/
                return View("Index", model);
            }

            var user = await _authService.ValidateAccount(model.Email!, model.Password!);
            if(user != null)
            {
                //HttpContext.User.
                UserAuthenticationModel userLogin = new UserAuthenticationModel
                {
                    IdUser = user.Id,
                    FullName = user.Fullname,
                    Role = user.Role.Rolename
                };
                string serializer = JsonConvert.SerializeObject(userLogin);
                HttpContext.Session.SetString(AppConfig.SESSION_USER ,serializer);
                return RedirectToAction("", "Home");
            }

            return View("Index", model);    
        }
    }
}
