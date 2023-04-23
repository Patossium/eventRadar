using Microsoft.AspNetCore.Mvc;

namespace eventRadar.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
