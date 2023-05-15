using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Identity;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using System.Security.Claims;

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<IEnumerable<UserDto>> GetMany()
        {
            var users = await _userRepository.GetManyAsync();
            return users.Select(o => new UserDto(o.Id, o.UserName, o.Email, o.PasswordHash, o.Name, o.Surname, o.LockoutEnd, o.LockoutEnabled));
        }
        [HttpGet()]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<UserDto>> Get()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);

            if(user == null)
            {
                return NotFound();
            }
            return new UserDto(user.Id, user.UserName, user.Email, user.PasswordHash, user.Name, user.Surname, user.LockoutEnd, user.LockoutEnabled);
        }
    }
}
