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
    [Route("api/user/VisitedEvent")]
    public class VisitedEventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IVisitedEventRepository _visitedEventRepository;
        private readonly IUserRepository _userRepository;

        public VisitedEventController(IEventRepository eventRepository, IAuthorizationService authorizationService, IVisitedEventRepository visitedEventRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _authorizationService = authorizationService;
            _visitedEventRepository = visitedEventRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        [Route("{eventId}")]
        public async Task<ActionResult<VisitedEventDto>> Create(int eventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            var eventObject = await _eventRepository.GetAsync(eventId);
            if (eventObject == null)
                return NotFound();

            var VisitedEvent = new VisitedEvent { Event = eventObject, EventId = eventId, User = user, UserId = userId };

            await _visitedEventRepository.CreateAsync(VisitedEvent);

            return Created("", new VisitedEventDto(VisitedEvent.Id, VisitedEvent.UserId, VisitedEvent.User, VisitedEvent.Event, VisitedEvent.EventId));
        }
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<VisitedEventDto>>> GetMany()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            var VisitedEvents = await _visitedEventRepository.GetManyAsync(user);

            return Ok(VisitedEvents.Select(o => new VisitedEventDto(o.Id, o.UserId, o.User, o.Event, o.EventId)));
        }
        [HttpGet()]
        [Route("{VisitedEventId}", Name = "GetVisitedEvent")]
        public async Task<ActionResult<VisitedEventDto>> Get(int VisitedEventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            var VisitedEvent = await _visitedEventRepository.GetAsync(user, VisitedEventId);
            if (VisitedEvent == null)
                return NotFound();

            return new VisitedEventDto(VisitedEvent.Id, VisitedEvent.UserId, VisitedEvent.User, VisitedEvent.Event, VisitedEvent.EventId);
        }
        [HttpGet()]
        [Route("{eventId}/check", Name = "CheckIfVisited")]
        public async Task<ActionResult<VisitedEventDto>> GetCheck(int eventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            var visitedEvent = await _visitedEventRepository.GetCheckAsync(user, eventId);
            if (visitedEvent == null)
                return NotFound();

            return new VisitedEventDto(visitedEvent.Id, visitedEvent.UserId, visitedEvent.User, visitedEvent.Event, visitedEvent.EventId);
        }
        [HttpDelete]
        [Route("{VisitedEventId}")]
        public async Task<ActionResult> Remove(int VisitedEventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            var VisitedEvent = await _visitedEventRepository.GetAsync(user, VisitedEventId);
            if (VisitedEvent == null)
                return NotFound();

            await _visitedEventRepository.DeleteAsync(VisitedEvent);

            return NoContent();
        }
        [HttpDelete]
        [Route("{eventId}/checked")]
        public async Task<ActionResult> RemoveByEvent(int eventId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await _userRepository.GetAsync(userId);
            var VisitedEvent = await _visitedEventRepository.GetCheckAsync(user, eventId);
            if (VisitedEvent == null)
                return NotFound();

            await _visitedEventRepository.DeleteAsync(VisitedEvent);

            return NoContent();
        }
    }
}
