using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
