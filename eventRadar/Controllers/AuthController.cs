using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Identity;
using eventRadar.Auth;
using static eventRadar.Auth.Model.AuthDtos;

namespace eventRadar.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(UserManager<User> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterUserDto registerUserDto)
        {
            var user = await _userManager.FindByNameAsync(registerUserDto.Username);
            if(user != null)
            {
                return BadRequest("Vartotojas su šiuo slapyvardžiu jau egzistuoja");
            }

            var newUser = new User
            {
                Email = registerUserDto.Email,
                UserName = registerUserDto.Username,
                Name = registerUserDto.Name,
                Surname = registerUserDto.Surname,
            };
            var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);
            if (!createUserResult.Succeeded)
                return BadRequest("Nepavyko sukurti vartotojo");

            await _userManager.AddToRoleAsync(newUser, SystemRoles.SystemUser);

            return CreatedAtAction(nameof(Register), new NewUserDto(newUser.Id, newUser.UserName, newUser.Email, newUser.Name, newUser.Surname));
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);

            if (user == null)
                return BadRequest("Neteisingi prisijungimo duomenys");

            if (user.LockoutEnd != null)
                return BadRequest("Naudotojas yra užblokuotas");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                return BadRequest("Neteisingi prisijungimo duomenys");

            // User is valid
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);

            return Ok(new SuccessfullLoginDto(accessToken));
        }
        [HttpPost]
        [Route("users/{userId}/block")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> BlockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) 
                return NotFound();

            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            return Ok(new UserDto(userId, user.UserName, user.Email, user.PasswordHash, user.Name, user.Surname, user.LockoutEnd, user.LockoutEnabled));
        }
        [HttpPost]
        [Route("users/{userId}/unblock")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> UnblockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);

            return Ok(new UserDto(userId, user.UserName, user.Email, user.PasswordHash, user.Name, user.Surname, user.LockoutEnd, user.LockoutEnabled));
        }
        [HttpPut]
        [Route("{userId}/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Validate the current password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.Password);

            if (!isPasswordValid)
            {
                return BadRequest("Invalid current password.");
            }

            // Change the user's password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.Password, changePasswordDto.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(changePasswordResult.Errors);
            }

            return NoContent();
        }
    }
}
