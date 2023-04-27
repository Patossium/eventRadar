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
                OldInformation = createChangedEventDto.OldInformation,
                NewInformation = createChangedEventDto.NewInformation,
                ChangeTime = DateTime.Now
            };
            await _changedEventRepository.CreateAsync(changedEvent);

            return Created("", new ChangedEventDto(changedEvent.Id, changedEvent.OldInformation, changedEvent.NewInformation, changedEvent.ChangeTime, changedEvent.Event));
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChangedEventDto>>> GetMany()
        {
            var changedEvents = await _changedEventRepository.GetManyAsync();

            return Ok(changedEvents.Select(o => new ChangedEventDto(o.Id, o.OldInformation, o.NewInformation, o.ChangeTime, o.Event)));
        }
        [HttpGet()]
        [Route("{changedEventId}", Name = "GetChangedEvent")]
        public async Task<ActionResult<ChangedEventDto>> Get(int changedEventId)
        {
            var changedEvent = await _changedEventRepository.GetAsync(changedEventId);
            if(changedEvent == null) 
                return NotFound();

            return new ChangedEventDto(changedEvent.Id, changedEvent.OldInformation, changedEvent.NewInformation, changedEvent.ChangeTime, changedEvent.Event);
        }
        [HttpDelete]
        [Route("{changedEventId}")]
        public async Task<ActionResult> Remove(int changedEventId)
        {
            var changedEvent = await _changedEventRepository.GetAsync(changedEventId);
            if(changedEvent== null)
                return NotFound();

            await _changedEventRepository.DeleteAsync(changedEvent);

            return NoContent();
        }
    }
}
