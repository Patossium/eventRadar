using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;
using eventRadar.Data;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ICategoryRepository _categoryRepository;
        public EventController(IEventRepository eventRepository, ILocationRepository locationRepository, ICategoryRepository categoryRepository)
        {
            _eventRepository = eventRepository;
            _locationRepository = locationRepository;
            _categoryRepository = categoryRepository;
        }
        [HttpGet]
        [Route("all")]
        public async Task<IEnumerable<EventDto>> GetMany()
        {
            var events = await _eventRepository.GetManyAsync();
            return events.Select(o => new EventDto(o.Id, o.Url, o.Title, o.DateStart, o.DateEnd, o.ImageLink, o.Price, o.TicketLink, o.Location, o.Category));
        }
        [HttpGet(Name = "GetEvents")]
        [Route("visi")]
        public async Task<IEnumerable<EventDto>> GetManyPaging([FromQuery] EventSearchParameters searchParameters)
        {
            var events = await _eventRepository.GetManyPagedAsync(searchParameters);

            var previousPageLink = events.HasPrevious ?
                CreateEventResourceUri(searchParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = events.HasNext ?
                CreateEventResourceUri(searchParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

            return events.Select(o => new EventDto(o.Id, o.Url, o.Title, o.DateStart, o.DateEnd, o.ImageLink, o.Price, o.TicketLink, o.Location, o.Category));
        }
        [HttpGet(Name = "GetFilteredEvents")]
        [Route("filtered/{Category}")]
        public async Task<IEnumerable<EventDto>> GetManyFilteredPaging([FromQuery] EventSearchParameters searchParameters, string Category)
        {
            var events = await _eventRepository.GetManyFilteredAsync(Category, searchParameters);
            var previousPageLink = events.HasPrevious ?
                CreateEventResourceUri(searchParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = events.HasNext ?
                CreateEventResourceUri(searchParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

            return events.Select(o => new EventDto(o.Id, o.Url, o.Title, o.DateStart, o.DateEnd, o.ImageLink, o.Price, o.TicketLink, o.Location, o.Category));
        }
        [HttpGet(Name = "GetSearchedEvents")]
        [Route("search/{search}")]
        public async Task<IEnumerable<EventDto>> GetManySearchedPaging([FromQuery] EventSearchParameters searchParameters, string search)
        {
            var events = await _eventRepository.GetManySearchedAsync(search, searchParameters);
            var previousPageLink = events.HasPrevious ?
                CreateEventResourceUri(searchParameters,
                ResourceUriType.PreviousPage) : null;

            var nextPageLink = events.HasNext ?
                CreateEventResourceUri(searchParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

            return events.Select(o => new EventDto(o.Id, o.Url, o.Title, o.DateStart, o.DateEnd, o.ImageLink, o.Price, o.TicketLink, o.Location, o.Category));
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

            return new EventDto(eventObject.Id, eventObject.Url, eventObject.Title, eventObject.DateStart, eventObject.DateEnd, eventObject.ImageLink,
                eventObject.Price, eventObject.TicketLink, eventObject.Location, eventObject.Category);
        }

        [HttpPost]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<EventDto>> Create(CreateEventDto createEventDto)
        {
            var eventObject = new Event
            {
                Url = createEventDto.Url,
                Title = createEventDto.Title,
                DateStart = createEventDto.DateStart,
                DateEnd = createEventDto.DateEnd,
                ImageLink = createEventDto.ImageLink,
                Price = createEventDto.Price,
                TicketLink = createEventDto.TicketLink,
                Location = createEventDto.Location,
                Category = createEventDto.Category
            };
            await _eventRepository.CreateAsync(eventObject);

            return Created("", new EventDto(eventObject.Id, eventObject.Url, eventObject.Title, eventObject.DateStart, eventObject.DateEnd, eventObject.ImageLink, eventObject.Price, 
                eventObject.TicketLink, eventObject.Location, eventObject.Category));
        }

        [HttpPut]
        [Route("{eventId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult<EventDto>> Update(int eventID, UpdateEventDto updateEventDto)
        {
            var eventObject = await _eventRepository.GetAsync(eventID);

            if (eventObject == null)
                return NotFound();

            eventObject.Title = updateEventDto.Title;
            eventObject.Url = updateEventDto.Url;
            eventObject.DateStart = updateEventDto.DateStart;
            eventObject.DateEnd = updateEventDto.DateEnd;
            eventObject.ImageLink = updateEventDto.ImageLink;
            eventObject.Price = updateEventDto.Price;
            eventObject.TicketLink = updateEventDto.TicketLink;
            eventObject.Location = updateEventDto.Location;
            eventObject.Category = updateEventDto.Category;

            await _eventRepository.UpdateAsync(eventObject);

            return Ok(new EventDto(eventID, eventObject.Url, eventObject.Title, eventObject.DateStart, eventObject.DateEnd, eventObject.ImageLink, 
                eventObject.Price, eventObject.TicketLink, eventObject.Location, eventObject.Category));
        }
        [HttpDelete]
        [Route("{eventId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<ActionResult> Remove(int eventId)
        {
            var eventObject = await _eventRepository.GetAsync(eventId);

            if (eventObject == null) 
                return NotFound();

            await _eventRepository.DeleteAsync(eventObject);

            return NoContent();
        }
        private string? CreateEventResourceUri(
            EventSearchParameters eventSearchParametersDto,
            ResourceUriType type
            )
        {
            return type switch
            {
                ResourceUriType.PreviousPage => Url.Link("GetEvents",
                new
                {
                    pageNumber = eventSearchParametersDto.PageNumber - 1,
                    pageSize = eventSearchParametersDto.PageSize,
                }),
                ResourceUriType.NextPage => Url.Link("GetEvents",
                new
                {
                    pageNumber = eventSearchParametersDto.PageNumber + 1,
                    pageSize = eventSearchParametersDto.PageSize,
                }),
                _ => Url.Link("GetEvents",
                new
                {
                    pageNumber = eventSearchParametersDto.PageNumber,
                    pageSize = eventSearchParametersDto.PageSize,
                })
            };
        }
    }
}
