using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class FollowedUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
