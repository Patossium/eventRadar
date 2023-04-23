using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class LocationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
