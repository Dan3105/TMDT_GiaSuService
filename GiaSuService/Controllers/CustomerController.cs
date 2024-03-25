using GiaSuService.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GiaSuService.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICatalogService _catalogService;
        //private readonly I

        public IActionResult Index()
        {
            return View();
        }
    }
}
