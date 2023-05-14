using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;
using eventRadar.Data;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Microsoft.IdentityModel.Tokens;
using ScrapySharp.Network;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Text;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using eventRadar.Helpers;

[assembly: InternalsVisibleTo("eventRadarUnitTests")]

namespace eventRadar.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {

        private readonly IEventRepository _eventRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IBlacklistedCategoryNameRepository _blacklistedCategoryNameRepository;
        private readonly IBlacklistedPageRepository _blacklistedPageRepository;
        public EventController(IEventRepository eventRepository, ILocationRepository locationRepository, ICategoryRepository categoryRepository, IWebsiteRepository websiteRepository,
            IBlacklistedCategoryNameRepository blacklistedCategoryNameRepository, IBlacklistedPageRepository blacklistedPageRepository)
        {
            _eventRepository = eventRepository;
            _locationRepository = locationRepository;
            _categoryRepository = categoryRepository;
            _websiteRepository = websiteRepository;
            _blacklistedPageRepository = blacklistedPageRepository;
            _blacklistedCategoryNameRepository = blacklistedCategoryNameRepository;
        }
        [HttpGet]
        [Route("all")]
        public async Task<IEnumerable<EventDto>> GetMany()
        {
            var events = await _eventRepository.GetManyAsync();
            return events.Select(o => new EventDto(o.Id, o.Url, o.Title, o.DateStart, o.DateEnd, o.ImageLink, o.Price, o.TicketLink, o.Location, o.Category));
        }
        [HttpGet(Name = "GetEvents")]
        [Route("completeList")]
        [Authorize(Roles = SystemRoles.Administrator)]
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
        [HttpGet(Name = "GetPastEvents")]
        [Route("allPast")]
        public async Task<IEnumerable<EventDto>> GetManyPastPaging([FromQuery] EventSearchParameters searchParameters)
        {
            var events = await _eventRepository.GetManyPastPagedAsync(searchParameters);

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
        [HttpGet(Name = "GetPastEvents")]
        [Route("allUpcoming")]
        public async Task<IEnumerable<EventDto>> GetManyUpcomingPaging([FromQuery] EventSearchParameters searchParameters)
        {
            var events = await _eventRepository.GetManyPastPagedAsync(searchParameters);

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
        [HttpGet(Name = "GetFilteredEvents")]
        [Route("filteredSearch/{Category}/{search}")]
        public async Task<IEnumerable<EventDto>> GetManyFilteredSearchPaging([FromQuery] EventSearchParameters searchParameters, string Category, string search)
        {
            var events = await _eventRepository.GetManyFilteredSearchAsync(search, Category, searchParameters);
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
        [HttpGet(Name = "GetPastFilteredEvents")]
        [Route("pastFilteredSearch/{Category}/{search}")]
        public async Task<IEnumerable<EventDto>> GetManyPastFilteredSearchPaging([FromQuery] EventSearchParameters searchParameters, string Category, string search)
        {
            var events = await _eventRepository.GetManyPastFilteredSearchAsync(search, Category, searchParameters);
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
        [Route("pastFiltered/{Category}")]
        public async Task<IEnumerable<EventDto>> GetManyPastFilteredPaging([FromQuery] EventSearchParameters searchParameters, string Category)
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
        [HttpGet(Name = "GetSearchedEvents")]
        [Route("pastSearch/{search}")]
        public async Task<IEnumerable<EventDto>> GetManyPastSearchedPaging([FromQuery] EventSearchParameters searchParameters, string search)
        {
            var events = await _eventRepository.GetManyPastSearchedAsync(search, searchParameters);
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
            try
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
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while creating the event.");
            }
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
            try
            {
                var eventObject = await _eventRepository.GetAsync(eventId);

                if (eventObject == null)
                    return NotFound();

                await _eventRepository.DeleteAsync(eventObject);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the event.");
            }
        }
        internal string? CreateEventResourceUri(
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

        private static ScrapingBrowser browser = new ScrapingBrowser();
        
        [HttpGet("event-details")]
        [Route("GetDetails/{websiteId}")]
        [Authorize(Roles = SystemRoles.Administrator)]
        public async Task<IEnumerable<EventDto>> GetEventDetails(int websiteId)
        {
            Website website = await _websiteRepository.GetAsync(websiteId);
            var listEventDetails = new List<Event>();
            var listBlacklistedCategory = await _blacklistedCategoryNameRepository.GetManyAsync();
            var listBlacklistedPages = await _blacklistedPageRepository.GetManyAsync();
            var blacklistedPages = new List<string>();
            foreach(var page in listBlacklistedPages)
            {
                blacklistedPages.Add(page.Url);
            }
            var listBlacklistedCategoryNames = new List<string>();
            foreach(var category in listBlacklistedCategory)
            {
                listBlacklistedCategoryNames.Add(category.Name);
            }
            var categoryList = ScraperHelper.GetCategories(website.Url, listBlacklistedCategoryNames, website);
            for(int i = 0; i < categoryList.Count; i++)
            {
                string firstLink = "";
                var html = ScraperHelper.GetHtml(categoryList[i].SourceUrl);
                var links = html.OwnerDocument.QuerySelectorAll(website.EventLink);
                foreach(var link in links)
                {
                    string UrlString = link.Attributes["href"].Value;
                    if (!blacklistedPages.Contains(UrlString))
                    {
                        firstLink = UrlString;
                        break;
                    }
                }
                var EventList = new List<Event>();
                var EventObject = new Event();
                Location location = ScraperHelper.GetLocationInfo(firstLink, website);
                EventObject.Location = location.Address + ", " + location.City + ", " + location.Country;
                EventObject.Location = "Test";
                EventObject.ImageLink = html.OwnerDocument.DocumentNode.SelectSingleNode(website.ImagePath).Attributes["src"].Value;
                EventObject.Url = categoryList[i].SourceUrl;
                string TempTitle = html.OwnerDocument.DocumentNode.SelectSingleNode(website.TitlePath).InnerText;
                TempTitle = Regex.Replace(TempTitle, @"^\s+|\s+$", "");
                TempTitle = Regex.Replace(TempTitle, "&quot;", "\"");
                TempTitle = Regex.Replace(TempTitle, "&amp;", "&");
                EventObject.Title = Regex.Replace(TempTitle, "&#039;", "'");
                EventObject.Category = categoryList[i].Name;
                if (html.OwnerDocument.DocumentNode.SelectSingleNode(website.PricePath) == null)
                {
                    EventObject.Price = "Tickets are not being sold";
                }
                else
                {
                    EventObject.Price = html.OwnerDocument.DocumentNode.SelectSingleNode(website.PricePath).InnerText;

                }
                string TempDate = html.OwnerDocument.DocumentNode.SelectSingleNode(website.DatePath).InnerText;
                TempDate = Regex.Replace(TempDate, "[a-zA-Z]+", "");
                TempDate = Regex.Replace(TempDate, "&#32;", " ");
                TempDate = Regex.Replace(TempDate, @"^\s+|\s+$", "");
                TempDate = Regex.Replace(TempDate, "[ąčęėįšųūžĄČĘĖĮŠŲŪŽ]", "");
                TempDate = Regex.Replace(TempDate, "  ", " ");

                if (TempDate.Contains(" - "))
                {
                    string[] dates = Regex.Split(TempDate, @"\s-\s");
                    EventObject.DateStart = DateTime.Parse(dates[0]);
                    EventObject.DateEnd = DateTime.Parse(dates[1]);

                }
                else
                {
                    EventObject.DateStart = DateTime.Parse(TempDate);
                    EventObject.DateEnd = DateTime.Parse(TempDate);
                }

                if (html.OwnerDocument.DocumentNode.SelectSingleNode(website.TicketPath) == null)
                {
                    EventObject.TicketLink = "";
                }
                else
                {
                    EventObject.TicketLink = html.OwnerDocument.DocumentNode.SelectSingleNode(website.TicketPath).Attributes[website.TicketLinkType].Value;
                }
                if (EventObject.Location != null)
                    listEventDetails.Add(EventObject);
            }
            return listEventDetails.Select(o => new EventDto(o.Id, o.Url, o.Title, o.DateStart, o.DateEnd, o.ImageLink, o.Price, o.TicketLink, o.Location, o.Category));
        }
    }
}
