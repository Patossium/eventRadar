using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eventRadar.Controllers;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            var result = actionResult.Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(EventDto));
            Assert.AreEqual(eventId, ((EventDto)result.Value).Id);
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
            var result = actionResult.Result as StatusCodeResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(500, result.StatusCode);
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
            var result = await _controller.Remove(eventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(500, ((StatusCodeResult)result).StatusCode);
        }
    }
}