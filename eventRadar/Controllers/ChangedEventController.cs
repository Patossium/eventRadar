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
    [Route("api/changedEvent")]
    public class ChangedEventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IChangedEventRepository _changedEventRepository;
        public ChangedEventController(IChangedEventRepository changedEventRepository, IEventRepository eventRepository)
        {
            _changedEventRepository = changedEventRepository;
            _eventRepository = eventRepository;
        }
        [HttpGet()]
        [Route("{eventId}", Name = "GetFakeEvent")]
        public async Task<ActionResult<EventDto>> Create(int eventId)
        {
            var eventObject = await _eventRepository.GetAsync(eventId);
            if (eventObject == null)
                return NotFound();

            return new EventDto(eventObject.Id, eventObject.Url, eventObject.Title, eventObject.Date, eventObject.ImageLink,
                eventObject.Price, eventObject.TicketLink, eventObject.Updated, eventObject.Location, eventObject.Category);
        }
        [HttpPost]
        [Route("{eventId}")]
        public async Task<ActionResult<ChangedEventDto>> Create(int eventId, CreateChangedEventDto createChangedEventDto)
        {
            var eventObject = await _eventRepository.GetAsync(eventId);
            if (eventObject == null) 
                return NotFound();

            var changedEvent = new ChangedEvent
            {
                Event = eventObject,
                NewInformation = createChangedEventDto.NewInformation,
                OldInformation = createChangedEventDto.OldInformation,
                ChangeTime = DateTime.Now
            };
            await _changedEventRepository.CreateAsync(changedEvent);

            return Created("", new ChangedEventDto(changedEvent.Id, changedEvent.OldInformation, changedEvent.NewInformation, changedEvent.ChangeTime, changedEvent.Event));
        }

    }
}
