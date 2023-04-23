using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        [HttpGet]
        public async Task<IEnumerable<EventDto>> GetMany()
        {
            var events = await _eventRepository.GetManyAsync();
            return events.Select(o => new EventDto(o.Id, o.Url, o.Title, o.Date, o.ImageLink, o.Price, o.TicketLink, o.Updated, o.Location, o.Category));
        }
        [HttpGet()]
        [Route("{eventId}", Name = "GetEvent")]
        public async Task<ActionResult<EventDto>> Get(int eventId)
        {
            var eventObject = await _eventRepository.GetAsync(eventId);
            if (eventObject == null)
            {
                return NotFound();
            }

            return new EventDto(eventObject.Id, eventObject.Url, eventObject.Title, eventObject.Date, eventObject.ImageLink, eventObject.Price, eventObject.TicketLink, eventObject.Updated, eventObject.Location, eventObject.Category);
        }
    }
}
