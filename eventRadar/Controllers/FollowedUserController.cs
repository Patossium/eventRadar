using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using System.Runtime.InteropServices;

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/user/{userID}/followedUser/")]
    public class FollowedUserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowedUserRepository _followedUserRepository;
        private readonly IAuthorizationService _authorizationService;

        public FollowedUserController(IUserRepository userRepository, IAuthorizationService authorizationService, IFollowedUserRepository followedUserRepository)
        {
            _userRepository = userRepository;
            _authorizationService = authorizationService;
            _followedUserRepository = followedUserRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowedUserDto>>> GetMany(int userId, int followedUserId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            var initialFollowedUser = await _userRepository.GetAsync(followedUserId);
            if(initialFollowedUser == null)
                return NotFound();

            var followedUser = await _followedUserRepository.GetManyAsync(user);
            return Ok(followedUser.Select(o => new FollowedUserDto(o.Id, o.UserId, o.User, o.Followed_User, o.FollowedUserId)));
        }
        [HttpGet()]
        [Route("{followedUserId}", Name = "GetFollowedUser")]
        public async Task<ActionResult<FollowedUserDto>> Get(int userId, int followedUserId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
                return NotFound();

            var followedUser = await _followedUserRepository.GetAsync(user, followedUserId);
            if(followedUser == null)
                return NotFound();

            return new FollowedUserDto(followedUser.Id, followedUser.UserId, followedUser.User, followedUser.Followed_User, followedUser.FollowedUserId);
        }

        [HttpPost]
        public async Task<ActionResult<FollowedUserDto>> Create(int userId, int followedUserId, CreateFollowedUserDto createFollowedUserDto)
        {
            var user = _userRepository.GetAsync(userId);
            if(user == null || user.Result == null) 
                return NotFound();

            var initialFollowedUser = _userRepository.GetAsync(followedUserId);
            if(initialFollowedUser == null || initialFollowedUser.Result == null)
                return NotFound();

            var followedUser = new FollowedUser
            {
                UserId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)),
                Followed_User = initialFollowedUser.Result
            };

            await _followedUserRepository.CreateAsync(followedUser);

            return Created("", new FollowedUserDto(followedUser.Id, followedUser.UserId, followedUser.User, followedUser.Followed_User, followedUser.FollowedUserId));
        }

        [HttpDelete]
        [Route("{followedUserId}")]
        public async Task<ActionResult> Remove(int userId, int initialFollowedUserId, int followedUserId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
                return NotFound();

            var initialFollowedUser = await _userRepository.GetAsync(initialFollowedUserId);
            if(initialFollowedUser == null)
                return NotFound();

            var followedUser = await _followedUserRepository.GetAsync(user, followedUserId);
            if(followedUser == null)
                return NotFound();

            await _followedUserRepository.DeleteAsync(followedUser);

            return NoContent();
        }
    }
}
