using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class FollowedEventController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
