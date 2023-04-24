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
            return user.Select(o => new UserDto(o.Id, o.Name, o.Email, o.Password, o.Lastname, o.Administrator, o.Blocked));
        }

        [HttpGet()]
        [Route("{userId}", Name = "GetUser")]
        public async Task<ActionResult<UserDto>> Get(int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
            {
                return NotFound();
            }
            return new UserDto(user.Id, user.Name, user.Email, user.Password, user.Lastname, user.Username, user.Administrator, user.Blocked);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<UserDto>> Create(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                Email = createUserDto.Email,
                Password = createUserDto.Password,
                Lastname = createUserDto.Lastname,
                Administrator = createUserDto.Administrator,
                Blocked = createUserDto.Blocked
            };
            await _userRepository.CreateAsync(user);
            return Created("", new UserDto(user.Id, user.Name, user.Email, user.Password, user.Username, user.Administrator, user.Blocked));
        }
        [HttpPut]
        [Route("{userId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<UserDto>> Update(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            user.Name = updateUserDto.Name;
            user.Email = updateUserDto.Email;
            user.Password = updateUserDto.Password;
            user.Username = updateUserDto.Username;
            user.Administrator = updateUserDto.Adminsitrator;
            user.Blocked = updateUserDto.Blocked;

            await _userRepository.UpdateAsync(user);
            return Ok(new UserDto(userId, user.Name, user.Email, user.Password, user.Lastname, user.Username, user.Administrator, user.Blocked));
        }

        [HttpDelete]
        [Route("{userId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(int userId)
        {
            var user = await _userRepository.GetAsync(userId);

            if(user == null)
                return NotFound();

            await _userRepository.DeleteAsync(user);

            return NoContent();
        }
    }
}
