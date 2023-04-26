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
        private readonly IChangedEventRepository _changedEventRepository;

        public ChangedEventController(IChangedEventRepository changedEventRepository)
        {
            _changedEventRepository = changedEventRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ChangedEventDto>> GetMany()
        {
            var changedEvent = await _changedEventRepository.GetManyAsync();
            return changedEvent.Select(o => new ChangedEventDto(o.Id, o.OldInformation, o.NewInformation, o.ChangeTime, o.Event));
        }

        [HttpGet()]
        [Route("{changedEventId}", Name = "GetChangedEvent")]
        public async Task<ActionResult<ChangedEventDto>> Get(int changedEventId)
        {
            var changedEvent = await _changedEventRepository.GetAsync(changedEventId);
            if(changedEvent == null)
            {
                return NotFound();
            }
            return new ChangedEventDto(changedEvent.Id, changedEvent.OldInformation, changedEvent.NewInformation, changedEvent.ChangeTime, changedEvent.Event);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<ChangedEventDto>> Create(CreateChangedEventDto createChangedEventDto)
        {
            var changedEvent = new ChangedEvent { OldInformation = createChangedEventDto.OldInformation, NewInformation = createChangedEventDto.NewInformation, ChangeTime = createChangedEventDto.ChangeTime, Event = createChangedEventDto.Event };
            await _changedEventRepository.CreateAsync(changedEvent);

            return Created("", new ChangedEventDto(changedEvent.Id, changedEvent.OldInformation, changedEvent.NewInformation, changedEvent.ChangeTime, changedEvent.Event));
        }

        [HttpPut]
        [Route("{changedEventId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<ChangedEventDto>> Update(int changedEventId, UpdateChangedEventDto updateChangedEventDto)
        {
            var changedEvent = await _changedEventRepository.GetAsync(changedEventId);

            if(changedEvent == null)
            {
                return NotFound();
            }
            changedEvent.OldInformation = updateChangedEventDto.OldInformation;
            changedEvent.NewInformation = updateChangedEventDto.NewInformation;
            changedEvent.ChangeTime = updateChangedEventDto.ChangeTime;

            await _changedEventRepository.UpdateAsync(changedEvent);

            return Ok(new ChangedEventDto(changedEventId, changedEvent.OldInformation, changedEvent.NewInformation, changedEvent.ChangeTime, changedEvent.Event));
        }

        [HttpDelete]
        [Route("{changedEventId}")]
        [Authorize(Roles =SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(int changedEventId)
        {
            var changedEvent = await _changedEventRepository.GetAsync(changedEventId);

            if(changedEvent == null)
            {
                return NotFound();
            }

            await _changedEventRepository.DeleteAsync(changedEvent);

            return NoContent();
        }
    }
}
