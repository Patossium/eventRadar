using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class BlacklistedPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
