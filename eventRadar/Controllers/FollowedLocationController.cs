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

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/user/{userId}/followedLocation")]
    public class FollowedLocationController : ControllerBase
    {
        private readonly IFollowedLocationRepository _followedLocationRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthorizationService _authorizationService;

        public FollowedLocationController(IFollowedLocationRepository followedLocationRepository, ILocationRepository locationRepository, IUserRepository userRepository, IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _followedLocationRepository = followedLocationRepository;
            _locationRepository = locationRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowedLocationDto>>> GetMany(string userId, string locationId)
        {
            var user = await _userRepository.GetAsync(userId);

            if (user == null)
                return NotFound();

            var location = await _locationRepository.GetAsync(locationId);

            if (location == null)
                return NotFound();

            var followedLocation = await _followedLocationRepository.GetManyAsync(user);

            return Ok(followedLocation.Select(o => new FollowedLocationDto(o.Id, o.UserId, o.User, o.Location, o.LocationId)));
        }

        [HttpGet()]
        [Route("{followedLocationId}", Name = "GetFollowedLocation")]
        public async Task<ActionResult<FollowedLocationDto>> Get(string userId, string locationId, string followedLocationId)
        {
            var user = await _userRepository.GetAsync(userId);

            if(user == null)
                return NotFound();

            var location = await _locationRepository.GetAsync(locationId);

            if(location == null)
                return NotFound();

            var followedLocation = await _followedLocationRepository.GetAsync(user, followedLocationId);

            if(followedLocation == null)
                return NotFound();

            return new FollowedLocationDto(followedLocation.Id, followedLocation.UserId, followedLocation.User, followedLocation.Location, followedLocation.LocationId);
        }

        [HttpPost]
        public async Task<ActionResult<FollowedLocationDto>> Create(string userId, string locationId, CreateFollowedLocationDto createFollowedLocationDto)
        {
            var user = _userRepository.GetAsync(userId);
            if (user == null || user.Result == null)
                return NotFound();
            
            var location = _locationRepository.GetAsync(locationId);
            if (location == null || location.Result == null)
                return NotFound();

            var followedLocation = new FollowedLocation { 
                UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub),
                Location = location.Result
            };

            await _followedLocationRepository.CreateAsync(followedLocation);

            return Created("", new FollowedLocationDto(followedLocation.Id, followedLocation.UserId, followedLocation.User, followedLocation.Location, followedLocation.LocationId));
        }

        [HttpDelete]
        [Route("{followedLocationId}")]
        public async Task<ActionResult> Remove(string userId, string locationId, string followedLocationId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
                return NotFound();

            var location = await _locationRepository.GetAsync(locationId);
            if(location == null)
                return NotFound();

            var followedLocation = await _followedLocationRepository.GetAsync(user, followedLocationId);
            if(followedLocation == null)
                return NotFound();

            await _followedLocationRepository.DeleteAsync(followedLocation);

            return NoContent();
        }
    }
}
