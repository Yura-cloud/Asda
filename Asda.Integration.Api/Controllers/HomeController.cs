using Microsoft.AspNetCore.Mvc;

namespace Asda.Integration.Api.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}