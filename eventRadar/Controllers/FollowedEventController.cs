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
    [Route("api/user/{userId}/FollowedEvents")]
    public class FollowedEventController : ControllerBase
    {
        private readonly IFollowedEventRepository _followedEventRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public FollowedEventController(IFollowedEventRepository followedEventRepository, IAuthorizationService authorizationService, IUserRepository userRepository, IEventRepository eventRepository)
        {
            _followedEventRepository = followedEventRepository;
            _authorizationService = authorizationService;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FollowedEventDto>>> GetMany(int userId)
        {
            var user = await _userRepository.GetAsync(userId);

            if(user == null)
            {
                return NotFound();
            }
            var followedEvents = await _followedEventRepository.GetManyAsync(user);
            return Ok(followedEvents.Select(o => new FollowedEventDto(o.Id, o.UserId, o.User, o.EventID, o.Event)));
        }

        [HttpGet()]
        [Route("{followedEventId}", Name = "GetFollowedEvent")]
        public async Task<ActionResult<FollowedEventDto>> Get(int userId, int followedEventId)
        {
            var user = await _userRepository.GetAsync(userId);

            if(user == null)
                return NotFound();

            var followedEvent = await _followedEventRepository.GetAsync(user, followedEventId);

            if(followedEvent == null)
                return NotFound();

            return new FollowedEventDto(followedEvent.Id, followedEvent.UserId, followedEvent.User, followedEvent.EventID, followedEvent.Event);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.SystemUser)]
        public async Task<ActionResult<FollowedEventDto>> Create(int eventId, CreateFollowedEventDto createFollowedEventDto)
        {
            var eventObject = _eventRepository.GetAsync(eventId);

            if(eventObject == null || eventObject.Result == null)
            {
                return NotFound();
            }
            var followedEvent = new FollowedEvent
            {
                Event = eventObject.Result,

            }
        }
    }
}
