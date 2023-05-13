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


namespace eventRadar.Tests
{
    [TestClass]
    public class EventControllerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<ILocationRepository> _locationRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly EventController _controller;

        public EventControllerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _controller = new EventController(_eventRepositoryMock.Object, _locationRepositoryMock.Object, _categoryRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetMany_ReturnsEvents()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Title = "Event 1" },
                new Event { Id = 2, Title = "Event 2" }
            };
            _eventRepositoryMock.Setup(repo => repo.GetManyAsync()).ReturnsAsync(events);

            // Act
            var result = await _controller.GetMany();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            CollectionAssert.AllItemsAreNotNull(result.ToList());
        }
        [TestMethod]
        public async Task Get_ReturnsEvent_WhenEventExists()
        {
            // Arrange
            int eventId = 1;
            var eventObject = new Event { Id = eventId, Title = "Event 1" };
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);

            // Act
            var actionResult = await _controller.Get(eventId);

            // Assert
            Assert.IsInstanceOfType(actionResult, typeof(ActionResult<EventDto>));
            var result = actionResult.Value as EventDto;
            Assert.IsNotNull(result);
            Assert.AreEqual(eventId, result.Id);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync((Event)null);

            // Act
            var actionResult = await _controller.Get(eventId);
            var result = actionResult.Result as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Create_ReturnsCreatedEvent_WhenEventIsValid()
        {
            // Arrange
            var createEventDto = new CreateEventDto("https://test.com", "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "https://test.com/image.png", "100", "https://test.com/tickets", "test location", "test category");
 
            _eventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);

            // Act
            var actionResult = await _controller.Create(createEventDto);
            var result = actionResult.Result as CreatedResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(EventDto));
            Assert.AreEqual(createEventDto.Title, ((EventDto)result.Value).Title);
        }
        [TestMethod]
        public async Task Create_ReturnsInternalServerError_WhenEventCreationFails()
        {
            // Arrange
            var createEventDto = new CreateEventDto("https://test.com", "Test Event", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "https://test.com/image.png", "100", "https://test.com/tickets", "test location", "test category");

            _eventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Event>()))
                .ThrowsAsync(new Exception("An error occurred while creating the event."));

            // Act
            var actionResult = await _controller.Create(createEventDto);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsInstanceOfType(actionResult.Result, typeof(ObjectResult));
            Assert.AreEqual(500, ((ObjectResult)actionResult.Result).StatusCode);
        }
        [TestMethod]
        public async Task Remove_ReturnsNoContent_WhenEventIsSuccessfullyDeleted()
        {
            // Arrange
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(new Event { Id = eventId });

            // Act
            var result = await _controller.Remove(eventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _eventRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Event>()), Times.Once());
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _controller.Remove(eventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            _eventRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Event>()), Times.Never());
        }

        [TestMethod]
        public async Task Remove_ReturnsInternalServerError_WhenEventDeletionFails()
        {
            // Arrange
            int eventId = 1;
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(new Event { Id = eventId });

            _eventRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<Event>()))
                .ThrowsAsync(new Exception("An error occurred while deleting the event."));

            // Act
            var result = await _controller.Remove(eventId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(500, result.StatusCode);
        }
        [TestMethod]
        public void CreateEventResourceUri_PreviousPage_ReturnsCorrectUri()
        {
            // Arrange
            var eventSearchParameters = new EventSearchParameters { PageNumber = 2, PageSize = 10 };
            var resourceUriType = ResourceUriType.PreviousPage;
            var expectedUri = "http://example.com/api/events?PageNumber=1&PageSize=10";

            // Setup UrlHelper mock
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(url => url.Link("GetEvents", It.IsAny<object>())).Returns(expectedUri);
            _controller.Url = urlHelperMock.Object;

            // Act
            var actualUri = _controller.CreateEventResourceUri(eventSearchParameters, resourceUriType);

            // Assert
            Assert.IsNotNull(actualUri);
            Assert.AreEqual(expectedUri, actualUri);
        }
        [TestMethod]
        public void CreateEventResourceUri_NextPage_ReturnsCorrectUri()
        {
            // Arrange
            var eventSearchParameters = new EventSearchParameters { PageNumber = 1, PageSize = 10 };
            var resourceUriType = ResourceUriType.NextPage;
            var expectedUri = "http://example.com/api/events?PageNumber=2&PageSize=10";

            // Setup UrlHelper mock
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(url => url.Link("GetEvents", It.IsAny<object>())).Returns(expectedUri);
            _controller.Url = urlHelperMock.Object;

            // Act
            var actualUri = _controller.CreateEventResourceUri(eventSearchParameters, resourceUriType);

            // Assert
            Assert.IsNotNull(actualUri);
            Assert.AreEqual(expectedUri, actualUri);
        }
        [TestMethod]
        public async Task Update_EventExists_ReturnsUpdatedEvent()
        {
            // Arrange
            int eventId = 1;
            var eventObject = new Event { Id = eventId, Title = "Original Title" };
            var updateEventDto = new UpdateEventDto ("test","Updated Title", DateTime.UtcNow, DateTime.UtcNow.AddHours(3), "test", "test", "test", "test", "test");

            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);
            _eventRepositoryMock.Setup(repo => repo.UpdateAsync(eventObject)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(eventId, updateEventDto);

            // Assert
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
            // Arrange
            int eventId = 1;
            var updateEventDto = new UpdateEventDto ("test", "Updated Title", DateTime.UtcNow, DateTime.UtcNow.AddHours(3), "test", "test", "test", "test", "test");

            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync((Event)null);

            // Act
            var result = await _controller.Update(eventId, updateEventDto);

            // Assert
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

            // Arrange
            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyPagedAsync(searchParameters)).ReturnsAsync(events);

            // Act
            var result = await _controller.GetManyPaging(searchParameters);

            // Assert
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
            // Arrange
            var searchParameters = new EventSearchParameters();
            var events = new PagedList<Event>(GetMockedEvents(), 1, 10, 100);
            _eventRepositoryMock.Setup(r => r.GetManyPagedAsync(searchParameters)).ReturnsAsync(events);

            var expectedPaginationMetadata = new
            {
                totalCount = events.TotalCount,
                pageSize = events.PageSize,
                currentPage = events.CurrentPage,
                totalPages = events.TotalPages,
                previousPageLink = (object)null, // Set expected previousPageLink value
                nextPageLink = (object)null // Set expected nextPageLink value
            };

            // Act
            var result = await _controller.GetManyPaging(searchParameters);

            // Assert
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
            // Arrange
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
                previousPageLink = (string)null, // Set expected previousPageLink value
                nextPageLink = (string)null // Set expected nextPageLink value
            };

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var urlHelperMock = new Mock<IUrlHelper>();
            _controller.Url = urlHelperMock.Object;

            urlHelperMock.Setup(x => x.Link("GetFilteredEvents", It.IsAny<object>())).Returns("testUrl");

            // Act
            var result = await _controller.GetManyFilteredPaging(searchParameters, category);

            // Assert
            Assert.IsTrue(_controller.Response.Headers.ContainsKey("Pagination"));
            var paginationHeaderValue = _controller.Response.Headers["Pagination"].ToString();
            var paginationMetadata = JsonSerializer.Deserialize<JsonElement>(paginationHeaderValue);
            Assert.AreEqual(expectedPaginationMetadata.totalCount, paginationMetadata.GetProperty("totalCount").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.pageSize, paginationMetadata.GetProperty("pageSize").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.currentPage, paginationMetadata.GetProperty("currentPage").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.totalPages, paginationMetadata.GetProperty("totalPages").GetInt32());
            Assert.AreEqual(expectedPaginationMetadata.previousPageLink, paginationMetadata.GetProperty("previousPageLink").GetString());
            Assert.AreEqual(expectedPaginationMetadata.nextPageLink, paginationMetadata.GetProperty("nextPageLink").GetString());

            // Additional assertions for the returned events
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
            // Arrange
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

            // Act
            var result = await _controller.GetManySearchedPaging(searchParameters, searchQuery);

            // Assert
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
            //Assert.AreEqual(searchedEvents.Count, ((Ok(result.Result).Value as IEnumerable<EventDto>).Count());
        }

        // Helper method to generate a list of mocked events
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