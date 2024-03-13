using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}
