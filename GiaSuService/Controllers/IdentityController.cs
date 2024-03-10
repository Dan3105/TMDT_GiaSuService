using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{
    public class IdentityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {

            return View();
        }
    }
}
