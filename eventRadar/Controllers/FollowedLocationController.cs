using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class FollowedLocationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
