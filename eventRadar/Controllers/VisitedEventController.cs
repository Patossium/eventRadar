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
    [Route("api/user/{userId}/visitedEvent")]
    public class VisitedEventController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IVisitedEventRepository _visitedEventRepository;

        public VisitedEventController(IUserRepository userRepository, IEventRepository eventRepository, IAuthorizationService authorizationService, IVisitedEventRepository visitedEventRepository)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _authorizationService = authorizationService;
            _visitedEventRepository = visitedEventRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitedEventDto>>> GetMany(int userId, int eventId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                return NotFound();

            var eventObject = await _eventRepository.GetAsync(eventId);
            if(eventObject == null) 
                return NotFound();

            var visitedEvent = await _visitedEventRepository.GetManyAsync(user);

            return Ok(visitedEvent.Select(o => new VisitedEventDto(o.Id, o.UserId, o.User, o.Event, o.EventId)));
        }

        [HttpGet()]
        [Route("{visitedEventId}", Name = "GetVisitedEvent")]
        public async Task<ActionResult<VisitedEventDto>> Get(int userId, int eventId, int visitedEventId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
                return NotFound();

            var eventObject = await _eventRepository.GetAsync(eventId);
            if(eventObject == null)
                return NotFound();

            var visitedEvent = await _visitedEventRepository.GetAsync(user, visitedEventId);
            if(visitedEvent == null) 
                return NotFound();

            return new VisitedEventDto(visitedEvent.Id, visitedEvent.UserId, visitedEvent.User, visitedEvent.Event, visitedEvent.EventId);
        }

        [HttpPost]
        public async Task<ActionResult<VisitedEventDto>> Create(int userId, int eventId, CreateVisitedEventDto createVisitedEventDto)
        {
            var user = _userRepository.GetAsync(userId);
            if(user == null || user.Result == null) 
                return NotFound();

            var eventObject = _eventRepository.GetAsync(eventId);
            if(eventObject == null || eventObject.Result == null)
                return NotFound();

            var visitedEvent = new VisitedEvent
            {
                UserId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)),
                Event = eventObject.Result,
                User = user.Result,
                EventId = eventId
            };

            await _visitedEventRepository.CreateAsync(visitedEvent);

            return Created("", new VisitedEventDto(visitedEvent.Id, visitedEvent.UserId, visitedEvent.User, visitedEvent.Event, visitedEvent.EventId));
        }

        [HttpDelete]
        [Route("{visitedEventId}")]
        public async Task<ActionResult> Remove(int userId, int eventId, int visitedEventId)
        {
            var user = await _userRepository.GetAsync(userId);
            if(user == null)
                return NotFound();

            var eventObject = await _eventRepository.GetAsync(eventId);
            if (eventObject == null)
                return NotFound();

            var visitedEvent = await _visitedEventRepository.GetAsync(user, visitedEventId);
            if(visitedEvent == null) 
                return NotFound();

            await _visitedEventRepository.DeleteAsync(visitedEvent);

            return NoContent();
        }
    }
}
