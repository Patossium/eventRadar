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
    [Route("api/user/followedEvent")]
    public class FollowedEventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IFollowedEventRepository _followedEventRepository;
        private readonly IUserRepository _userRepository;

        public FollowedEventController(IEventRepository eventRepository, IAuthorizationService authorizationService, IFollowedEventRepository followedEventRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _authorizationService = authorizationService;
            _followedEventRepository = followedEventRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("{eventId}")]
        public async Task<ActionResult<FollowedEventDto>> Create(int eventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            var eventObject = await _eventRepository.GetAsync(eventId);
            if (eventObject == null)
                return NotFound();

            var followedEvent = new FollowedEvent { Event = eventObject, EventId = eventId, User = user, UserId = userId };

            await _followedEventRepository.CreateAsync(followedEvent);

            return Created("", new FollowedEventDto(followedEvent.Id, followedEvent.UserId, followedEvent.User, followedEvent.Event, followedEvent.EventId));
        }
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<FollowedEventDto>>> GetMany()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
                return NotFound();

            var followedEvents = await _followedEventRepository.GetManyAsync(user);

            return Ok(followedEvents.Select(o => new FollowedEventDto(o.Id, o.UserId, o.User, o.Event, o.EventId)));
        }
        [HttpGet()]
        [Route("{followedEventId}", Name ="GetFollowedEvent")]
        public async Task<ActionResult<FollowedEventDto>> Get(int followedEventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            var followedEvent = await _followedEventRepository.GetAsync(user, followedEventId);
            if(followedEvent == null)
                return NotFound();

            return new FollowedEventDto(followedEvent.Id, followedEvent.UserId, followedEvent.User, followedEvent.Event, followedEvent.EventId);
        }
        [HttpDelete]
        [Route("{followedEventId}")]
        public async Task<ActionResult> Remove(int followedEventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            var followedEvent = await _followedEventRepository.GetAsync(user, followedEventId);
            if(followedEvent == null)
                return NotFound();

            await _followedEventRepository.DeleteAsync(followedEvent);

            return NoContent();
        }
    }
}
