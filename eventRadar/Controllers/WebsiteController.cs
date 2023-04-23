using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class WebsiteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
