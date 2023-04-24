using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;

namespace eventRadar.Controllers
{
    public class FollowedLocationController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
