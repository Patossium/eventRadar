using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetMany()
        {
            var user = await _userRepository.GetManyAsync();
            return user.Select(o => new UserDto(o.Id, o.UserName, o.Email, o.PasswordHash, o.Name, o.Surname, o.Blocked));
        }

        [HttpGet()]
        [Route("{userId}", Name = "GetUser")]
        public async Task<ActionResult<UserDto>> Get(string userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            return new UserDto(user.Id, user.UserName, user.Email, user.PasswordHash, user.Name, user.Surname, user.Blocked);
        }

        [HttpPut]
        [Route("{userId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<UserDto>> BlockUser(string userId, BlockUserDto blockUserDto)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            user.Blocked = true;

            await _userRepository.UpdateAsync(user);
            return Ok(new UserDto(userId, user.UserName, user.Email, user.PasswordHash, user.Name, user.Surname, user.Blocked));
        }

        [HttpPut]
        [Route("{userId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<UserDto>> UnblockUser(string userId, BlockUserDto blockUserDto)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            user.Blocked = false;

            await _userRepository.UpdateAsync(user);
            return Ok(new UserDto(userId, user.UserName, user.Email, user.PasswordHash, user.Name, user.Surname, user.Blocked));
        }
    }
}
