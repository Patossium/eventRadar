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
    [Route("api/user/{userId}/followedEvent/")]
    public class FollowedEventController : ControllerBase
    {
        private readonly IFollowedEventRepository _followedEventRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuthorizationService _authorizationService;

        public FollowedEventController(IUserRepository userRepository, IEventRepository eventRepository, IAuthorizationService authorizationService, IFollowedEventRepository followedEventRepository)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _authorizationService = authorizationService;
            _followedEventRepository = followedEventRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowedEventDto>>> GetMany(string userId, string eventId)
        {
            var user = await _userRepository.GetAsync(userId);

            if(user == null)
            {
                return NotFound();
            }

            var eventObject = await _eventRepository.GetAsync(eventId);

            if(eventObject == null)
            {
                return NotFound();
            }

            var followedEvent = await _followedEventRepository.GetManyAsync(user);
            return Ok(followedEvent.Select(o => new FollowedEventDto(o.Id, o.UserId, o.User, o.Event, o.EventId)));
        }

        [HttpGet()]
        [Route("{followedEventId}", Name = "GetFollowedEvent")]
        public async Task<ActionResult<FollowedEventDto>> Get(string userId, string eventId, string followedEventId)
        {
            var user = await _userRepository.GetAsync(userId);

            if(user == null)
                return NotFound();

            var eventObject = await _eventRepository.GetAsync(eventId);

            if (eventObject == null)
                return NotFound();

            var followedEvent = await _followedEventRepository.GetAsync(user, followedEventId);

            if(followedEvent == null)
                return NotFound();

            return new FollowedEventDto(followedEvent.Id, followedEvent.UserId, followedEvent.User, followedEvent.Event, followedEvent.EventId);
        }

        [HttpPost]
        public async Task<ActionResult<FollowedEventDto>> Create(string userId, string eventId, CreateFollowedEventDto createFollowedEventDto)
        {
            var user = _userRepository.GetAsync(userId);
            if(user == null || user.Result == null)
                return NotFound();

            var eventObject = _eventRepository.GetAsync(eventId);
            if(eventObject == null || eventObject.Result == null)
                return NotFound();

            var followedEvent = new FollowedEvent { 
                UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub), 
                Event = eventObject.Result,
                User = user.Result,
                EventId = eventId
            };

            await _followedEventRepository.CreateAsync(followedEvent);

            return Created("", new FollowedEventDto(followedEvent.Id, followedEvent.UserId, followedEvent.User, followedEvent.Event, followedEvent.EventId));
        }

        [HttpDelete]
        [Route("{folowedEventId}")]
        public async Task<ActionResult> Remove(string userId, string eventId, string followedEventId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null) 
                return NotFound();

            var eventObject = await _eventRepository.GetAsync(eventId);
            if(eventObject == null) 
                return NotFound();

            var followedEvent = await _followedEventRepository.GetAsync(user, followedEventId);
            if(followedEvent == null) 
                return NotFound();

            await _followedEventRepository.DeleteAsync(followedEvent);
            
            return NoContent();
        }
    }
}
