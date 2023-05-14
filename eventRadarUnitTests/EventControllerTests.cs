using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eventRadar.Controllers;
using eventRadar.Data;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Helpers;
using eventRadar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using HtmlAgilityPack;


namespace eventRadar.Tests
{
    [TestClass]
    public class EventControllerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<ILocationRepository> _locationRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IWebsiteRepository> _websiteRepositoryMock;
        private readonly Mock<IBlacklistedCategoryNameRepository> _blacklistedCategoryNameRepositoryMock;
        private readonly Mock<IBlacklistedPageRepository> _blacklistedPageRepositoryMock;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _websiteRepositoryMock = new Mock<IWebsiteRepository>();
            _blacklistedCategoryNameRepositoryMock = new Mock<IBlacklistedCategoryNameRepository>();
            _blacklistedPageRepositoryMock = new Mock<IBlacklistedPageRepository>();
            _controller = new EventController(
                _eventRepositoryMock.Object,
                _locationRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _websiteRepositoryMock.Object,
                _blacklistedCategoryNameRepositoryMock.Object,
                _blacklistedPageRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetMany_ReturnsEvents()
        {
            var events = new List<Event>
            {
                new Event { Id = 1, Title = "Event 1" },
                new Event { Id = 2, Title = "Event 2" }
            };
            _eventRepositoryMock.Setup(repo => repo.GetManyAsync()).ReturnsAsync(events);

            var result = await _controller.GetMany();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            CollectionAssert.AllItemsAreNotNull(result.ToList());
        }
        [TestMethod]
        public async Task Get_ReturnsEvent_WhenEventExists()
        {
            int eventId = 1;
            var eventObject = new Event { Id = eventId, Title = "Event 1" };
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);

            var actionResult = await _controller.Get(eventId);

            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<EventDto>));
            var result = actionResult.Value as EventDto;
            Assert.IsNotNull(result);
            Assert.AreEqual(eventId, result.Id);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenEventDoesNotExist()
        {
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync((Event)null);

            var actionResult = await _controller.Get(eventId);
            var result = actionResult.Result as NotFoundResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Create_ReturnsCreatedEvent_WhenEventIsValid()
        {
            var createEventDto = new CreateEventDto("https://test.com", "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "https://test.com/image.png", "100", "https://test.com/tickets", "test location", "test category");
 
            _eventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);

            var actionResult = await _controller.Create(createEventDto);
            var result = actionResult.Result as CreatedResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(EventDto));
            Assert.AreEqual(createEventDto.Title, ((EventDto)result.Value).Title);
        }
        [TestMethod]
        public async Task Create_ReturnsInternalServerError_WhenEventCreationFails()
        {
            var createEventDto = new CreateEventDto("https://test.com", "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "https://test.com/image.png", "100", "https://test.com/tickets", "test location", "test category");

            _eventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Event>()))
                .ThrowsAsync(new Exception("An error occurred while creating the event."));

            var actionResult = await _controller.Create(createEventDto);

            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Result, typeof(ObjectResult));
            Assert.AreEqual(500, ((ObjectResult)actionResult.Result).StatusCode);
        }
        [TestMethod]
        public async Task Remove_ReturnsNoContent_WhenEventIsSuccessfullyDeleted()
        {
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(new Event { Id = eventId });

            var result = await _controller.Remove(eventId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _eventRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Event>()), Times.Once());
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFound_WhenEventDoesNotExist()
        {
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync((Event)null);

            var result = await _controller.Remove(eventId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _eventRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Event>()), Times.Never());
        }

        [TestMethod]
        public async Task Remove_ReturnsInternalServerError_WhenEventDeletionFails()
        {
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(new Event { Id = eventId });

            _eventRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Event>()))
                .ThrowsAsync(new Exception("An error occurred while deleting the event."));

            var result = await _controller.Remove(eventId) as ObjectResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(500, result.StatusCode);
        }
        [TestMethod]
        public void CreateEventResourceUri_PreviousPage_ReturnsCorrectUri()
        {
            var eventSearchParameters = new EventSearchParameters { PageNumber = 2, PageSize = 10 };
            var resourceUriType = ResourceUriType.PreviousPage;
            var expectedUri = "http://example.com/api/events?PageNumber=1&PageSize=10";

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(url => url.Link("GetEvents", It.IsAny<object>())).Returns(expectedUri);
            _controller.Url = urlHelperMock.Object;

            var actualUri = _controller.CreateEventResourceUri(eventSearchParameters, resourceUriType);

            Assert.IsNotNull(actualUri);
            Assert.AreEqual(expectedUri, actualUri);
        }
        [TestMethod]
        public void CreateEventResourceUri_NextPage_ReturnsCorrectUri()
        {
            var eventSearchParameters = new EventSearchParameters { PageNumber = 1, PageSize = 10 };
            var resourceUriType = ResourceUriType.NextPage;
            var expectedUri = "http://example.com/api/events?PageNumber=2&PageSize=10";

            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(url => url.Link("GetEvents", It.IsAny<object>())).Returns(expectedUri);
            _controller.Url = urlHelperMock.Object;

            var actualUri = _controller.CreateEventResourceUri(eventSearchParameters, resourceUriType);

            Assert.IsNotNull(actualUri);
            Assert.AreEqual(expectedUri, actualUri);
        }
        [TestMethod]
        public async Task Update_EventExists_ReturnsUpdatedEvent()
        {
            int eventId = 1;
            var eventObject = new Event { Id = eventId, Title = "Original Title" };
            var updateEventDto = new UpdateEventDto ("test","Updated Title", DateTime.UtcNow, DateTime.UtcNow.AddHours(3), "test", "test", "test", "test", "test");

            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);
            _eventRepositoryMock.Setup(repo => repo.UpdateAsync(eventObject)).Returns(Task.CompletedTask);

            var result = await _controller.Update(eventId, updateEventDto);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult.Value, typeof(EventDto));
            var eventDto = okResult.Value as EventDto;
            Assert.AreEqual(updateEventDto.Title, eventDto.Title);
        }

        [TestMethod]
        public async Task Update_EventDoesNotExist_ReturnsNotFound()
        {
            int eventId = 1;
            var updateEventDto = new UpdateEventDto ("test", "Updated Title", DateTime.UtcNow, DateTime.UtcNow.AddHours(3), "test", "test", "test", "test", "test");

            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync((Event)null);

            var result = await _controller.Update(eventId, updateEventDto);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task GetManyPaging_ReturnsEventDtoList()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyPagedAsync(searchParameters)).ReturnsAsync(events);

            var result = await _controller.GetManyPaging(searchParameters);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<EventDto>));

            if (result != null)
            {
                var eventDtoList = result.ToList();
                Assert.AreEqual(events.Count, eventDtoList.Count);
            }
        }
        [TestMethod]
        public async Task GetManyPastPaging_ReturnsEventDtoList()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyPastPagedAsync(searchParameters)).ReturnsAsync(events);

            var result = await _controller.GetManyPastPaging(searchParameters);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<EventDto>));

            if (result != null)
            {
                var eventDtoList = result.ToList();
                Assert.AreEqual(events.Count, eventDtoList.Count);
            }
        }
        [TestMethod]
        public async Task GetManyUpcomingPaging_ReturnsEventDtoList()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyUpcomingPagedAsync(searchParameters)).ReturnsAsync(events);

            var result = await _controller.GetManyUpcomingPaging(searchParameters);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<EventDto>));

            if (result != null)
            {
                var eventDtoList = result.ToList();
                Assert.AreEqual(events.Count, eventDtoList.Count);
            }
        }
        [TestMethod]
        public async Task GetManyPaging_SetsPaginationHeaders()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyPagedAsync(searchParameters)).ReturnsAsync(events);

            var expectedPaginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink = (object)null,
                nextPageLink = (object)null
            };

            var result = await _controller.GetManyPaging(searchParameters);

            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());
        }
        [TestMethod]
        public async Task GetManyPastPaging_SetsPaginationHeaders()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyPastPagedAsync(searchParameters)).ReturnsAsync(events);

            var expectedPaginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink = (object)null,
                nextPageLink = (object)null
            };

            var result = await _controller.GetManyPastPaging(searchParameters);

            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());
        }
        [TestMethod]
        public async Task GetManyUpcomingPaging_SetsPaginationHeaders()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyUpcomingPagedAsync(searchParameters)).ReturnsAsync(events);

            var expectedPaginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink = (object)null,
                nextPageLink = (object)null
            };

            var result = await _controller.GetManyUpcomingPaging(searchParameters);

            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());
        }
        [TestMethod]
        public async Task GetManyFilteredPaging_ReturnsFilteredEventsWithPaginationMetadata()
        {
            var searchParameters = new EventSearchParameters();
            var category = "TestCategory";
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyFilteredAsync(category, searchParameters)).ReturnsAsync(events);

            var expectedPaginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink = (string)null,
                nextPageLink = (string)null
            };

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var urlHelperMock = new Mock<IUrlHelper>();
            _controller.Url = urlHelperMock.Object;

            urlHelperMock.Setup(x => x.Link("GetFilteredEvents", It.IsAny<object>())).Returns("testUrl");

            var result = await _controller.GetManyFilteredPaging(searchParameters, category);

            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());

            var eventDtos = result.ToList();
            Assert.AreEqual(events.Count(), eventDtos.Count);
        }
        [TestMethod]
        public async Task GetManyPastFilteredPaging_ReturnsFilteredEventsWithPaginationMetadata()
        {
            var searchParameters = new EventSearchParameters();
            var category = "TestCategory";
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyPastFilteredAsync(category, searchParameters)).ReturnsAsync(events);

            var expectedPaginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink = (string)null,
                nextPageLink = (string)null
            };

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var urlHelperMock = new Mock<IUrlHelper>();
            _controller.Url = urlHelperMock.Object;

            urlHelperMock.Setup(x => x.Link("GetFilteredEvents", It.IsAny<object>())).Returns("testUrl");

            var result = await _controller.GetManyPastFilteredPaging(searchParameters, category);

            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());

            var eventDtos = result.ToList();
            Assert.AreEqual(events.Count(), eventDtos.Count);
        }
        [TestMethod]
        public async Task GetManyUpcomingFilteredPaging_ReturnsFilteredEventsWithPaginationMetadata()
        {
            var searchParameters = new EventSearchParameters();
            var category = "TestCategory";
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyUpcomingFilteredAsync(category, searchParameters)).ReturnsAsync(events);

            var expectedPaginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink = (string)null,
                nextPageLink = (string)null
            };

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var urlHelperMock = new Mock<IUrlHelper>();
            _controller.Url = urlHelperMock.Object;

            urlHelperMock.Setup(x => x.Link("GetFilteredEvents", It.IsAny<object>())).Returns("testUrl");

            var result = await _controller.GetManyUpcomingFilteredPaging(searchParameters, category);

            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());

            var eventDtos = result.ToList();
            Assert.AreEqual(events.Count(), eventDtos.Count);
        }
        [TestMethod]
        public async Task GetManySearchedPaging_ReturnsSearchedEventsWithPaginationMetadata()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchQuery = "test";
            var searchParameters = new EventSearchParameters();
            var searchedEvents = GetMockedEvents();

            _eventRepositoryMock.Setup(r => r.GetManySearchedAsync(searchQuery, searchParameters))
                .ReturnsAsync(new PagedList<Event>(searchedEvents, 2, 1, searchedEvents.Count));

            var expectedPaginationMetadata = new
            {
                totalCount = searchedEvents.Count,
                pageSize = searchedEvents.Count,
                currentPage = 1,
                totalPages = 1,
                previousPageLink = (object)null,
                nextPageLink = (object)null
            };

            var result = await _controller.GetManySearchedPaging(searchParameters, searchQuery);

            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());
        }
       [TestMethod]
        public async Task GetManyFilteredSearchPaging_ReturnsFilteredEvents()
        {
            try
            {
                // Arrange
                var searchParameters = new EventSearchParameters();
                var category = "Event 1 Category";
                var search = "TestSearch";
                var events = new List<Event>()
                {
                    new Event { Id = 1, Title = "Event 1" },
                    new Event { Id = 2, Title = "Event 2" },
                };

                _eventRepositoryMock.Setup(repo => repo.GetManyFilteredSearchAsync(search, category, searchParameters))
                    .ReturnsAsync(new PagedList<Event>(events, events.Count, searchParameters.PageSize, 1));

                // Act
                var result = await _controller.GetManyFilteredSearchPaging(searchParameters, category, search);

                // Assert
                Assert.IsNotNull(result);
                var eventDtos = result.ToList();
                Assert.AreEqual(2, eventDtos.Count);
                Assert.AreEqual(1, eventDtos[0].Id);
                Assert.AreEqual("Event 1", eventDtos[0].Title);
                Assert.AreEqual(2, eventDtos[1].Id);
                Assert.AreEqual("Event 2", eventDtos[1].Title);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Test failed with exception: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

        [TestMethod]
        public async Task GetManyPastSearchedPaging_ReturnsSearchedEventsWithPaginationMetadata()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchQuery = "test";
            var searchParameters = new EventSearchParameters();
            var searchedEvents = GetMockedEvents();

            _eventRepositoryMock.Setup(r => r.GetManyPastSearchedAsync(searchQuery, searchParameters))
                .ReturnsAsync(new PagedList<Event>(searchedEvents, 2, 1, searchedEvents.Count));

            var expectedPaginationMetadata = new
            {
                totalCount = searchedEvents.Count,
                pageSize = searchedEvents.Count,
                currentPage = 1,
                totalPages = 1,
                previousPageLink = (object)null,
                nextPageLink = (object)null
            };

            var result = await _controller.GetManyPastSearchedPaging(searchParameters, searchQuery);

            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());
        }
        [TestMethod]
        public async Task GetManyUpcomingSearchedPaging_ReturnsSearchedEventsWithPaginationMetadata()
        {
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var UrlHelperMock = new Mock<IUrlHelper>();

            _controller.Url = UrlHelperMock.Object;

            UrlHelperMock.Setup(x => x.Link("GetAnyEvents", It.IsAny<Object>)).Returns("testUrl");

            var searchQuery = "test";
            var searchParameters = new EventSearchParameters();
            var searchedEvents = GetMockedEvents();

            _eventRepositoryMock.Setup(r => r.GetManyUpcomingSearchedAsync(searchQuery, searchParameters))
                .ReturnsAsync(new PagedList<Event>(searchedEvents, 2, 1, searchedEvents.Count));

            var expectedPaginationMetadata = new
            {
                totalCount = searchedEvents.Count,
                pageSize = searchedEvents.Count,
                currentPage = 1,
                totalPages = 1,
                previousPageLink = (object)null,
                nextPageLink = (object)null
            };

            var result = await _controller.GetManyUpcomingSearchedPaging(searchParameters, searchQuery);

            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());
        }
        private List<Event> GetMockedEvents()
        {
            var events = new List<Event>
    {
        new Event
        {
            Id = 1,
            Url = "https://example.com/event1",
            Title = "Event 1 test",
            DateStart = DateTime.UtcNow,
            DateEnd = DateTime.UtcNow.AddDays(1),
            ImageLink = "https://example.com/event1-image.jpg",
            Price = "10.99",
            TicketLink = "https://example.com/event1-tickets",
            Location = "Event 1 Location",
            Category = "Event 1 Category"
        },
        new Event
        {
            Id = 2,
            Url = "https://example.com/event2",
            Title = "Event 2 test",
            DateStart = DateTime.UtcNow.AddDays(2),
            DateEnd = DateTime.UtcNow.AddDays(3),
            ImageLink = "https://example.com/event2-image.jpg",
            Price = "15.99",
            TicketLink = "https://example.com/event2-tickets",
            Location = "Event 2 Location",
            Category = "Event 2 Category"
        },
        };

            return events;
        }
    }
}