using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using eventRadar.Controllers;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Models;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace eventRadar.Tests
{
    [TestClass]
    public class VisitedEventControllerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;
        private readonly Mock<IVisitedEventRepository> _VisitedEventRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly VisitedEventController _controller;

        public VisitedEventControllerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _VisitedEventRepositoryMock = new Mock<IVisitedEventRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _controller = new VisitedEventController(
                _eventRepositoryMock.Object,
                _authorizationServiceMock.Object,
                _VisitedEventRepositoryMock.Object,
                _userRepositoryMock.Object);

            var userIdentity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(JwtRegisteredClaimNames.Sub, "testuserid")
            });
            var claimsPrincipal = new ClaimsPrincipal(userIdentity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedAtActionResult_WhenVisitedEventIsCreated()
        {
            int eventId = 1;
            var user = new User { Id = "testuserid" };
            var eventObject = new Event { Id = eventId };
            var VisitedEvent = new VisitedEvent { Id = 1, UserId = "testuserid", User = user, EventId = eventId, Event = eventObject };

            _userRepositoryMock.Setup(repo => repo.GetAsync("testuserid")).ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);
            _VisitedEventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<VisitedEvent>()))
                .Callback((VisitedEvent fe) =>
                {
                    fe.Id = 1;
                });

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdResult = result.Result as CreatedResult;
            Assert.IsNotNull(createdResult.Value);
            Assert.IsInstanceOfType(createdResult.Value, typeof(VisitedEventDto));
            var VisitedEventDto = createdResult.Value as VisitedEventDto;
            Assert.AreEqual(VisitedEvent.Id, VisitedEventDto.Id);
            Assert.AreEqual(VisitedEvent.UserId, VisitedEventDto.userId);
            Assert.AreEqual(VisitedEvent.User, VisitedEventDto.User);
            Assert.AreEqual(VisitedEvent.EventId, VisitedEventDto.EventId);
            Assert.AreEqual(VisitedEvent.Event, VisitedEventDto.Event);
        }

        [TestMethod]
        public async Task Create_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            var eventId = 1;
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync((User)null);

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ReturnsNotFound_WhenEventNotFound()
        {
            var userId = "existingUserId";
            var eventId = 1;
            var user = new User { Id = userId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync((Event)null);

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Create_CallsVisitedEventRepositoryCreateAsync()
        {
            var eventId = 1;
            var user = new User { Id = "testuserid" };
            var eventObject = new Event { Id = eventId };
            _userRepositoryMock.Setup(repo => repo.GetAsync("testuserid"))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(eventObject);
            var VisitedEvent = new VisitedEvent();
            _VisitedEventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<VisitedEvent>()))
                .Callback((VisitedEvent fe) => VisitedEvent = fe)
                .Returns(Task.CompletedTask);

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdResult = result.Result as CreatedResult;
            Assert.IsNotNull(createdResult.Value);
            Assert.IsInstanceOfType(createdResult.Value, typeof(VisitedEventDto));
            var VisitedEventDto = createdResult.Value as VisitedEventDto;
            Assert.AreEqual(VisitedEvent.Id, VisitedEventDto.Id);
            Assert.AreEqual(VisitedEvent.UserId, VisitedEventDto.userId);
            Assert.AreEqual(VisitedEvent.User, VisitedEventDto.User);
            Assert.AreEqual(VisitedEvent.Event, VisitedEventDto.Event);
            Assert.AreEqual(VisitedEvent.EventId, VisitedEventDto.EventId);
            _VisitedEventRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<VisitedEvent>()), Times.Once());
        }
        [TestMethod]
        public async Task Create_ReturnsInternalServerError_WhenVisitedEventCreationFails()
        {
            var userId = "existingUserId";
            var eventId = 1;
            var user = new User { Id = userId };
            var eventObject = new Event { Id = eventId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(eventObject);
            _VisitedEventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<VisitedEvent>()))
                .ThrowsAsync(new Exception("Failed to create followed event"));

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<VisitedEventDto>));
            var objectResult = result.Result;
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            var eventId = 1;
            _controller.ModelState.AddModelError("EventId", "The EventId field is required.");

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            var notFoundResult = result.Result as NotFoundResult;
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }
        [TestMethod]
        public async Task GetMany_ReturnsOkObjectResult_WhenUserHasVisitedEvents()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var VisitedEvents = new List<VisitedEvent>
            {
                new VisitedEvent { Id = 1, UserId = userId, EventId = 1 },
                new VisitedEvent { Id = 2, UserId = userId, EventId = 2 }
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetManyAsync(user)).ReturnsAsync(VisitedEvents);

            var result = await _controller.GetMany();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var VisitedEventsDto = okResult.Value as IEnumerable<VisitedEventDto>;
            Assert.AreEqual(2, VisitedEventsDto.Count());
        }

        [TestMethod]
        public async Task GetMany_ReturnsOkObjectResult_WhenUserHasNoVisitedEvents()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var VisitedEvents = new List<VisitedEvent>();

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetManyAsync(user)).ReturnsAsync(VisitedEvents);

            var result = await _controller.GetMany();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var VisitedEventsDto = okResult.Value as IEnumerable<VisitedEventDto>;
            Assert.AreEqual(0, VisitedEventsDto.Count());
        }

        [TestMethod]
        public async Task GetMany_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.GetMany();

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Get_ReturnsVisitedEventDto_WhenVisitedEventExists()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var VisitedEventId = 1;
            var VisitedEvent = new VisitedEvent { Id = VisitedEventId, UserId = userId, EventId = 1 };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, VisitedEventId)).ReturnsAsync(VisitedEvent);

            var result = await _controller.Get(VisitedEventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(VisitedEventDto));
            var VisitedEventDto = result.Value;
            Assert.AreEqual(VisitedEvent.Id, VisitedEventDto.Id);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenVisitedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var VisitedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, VisitedEventId)).ReturnsAsync((VisitedEvent)null);

            var result = await _controller.Get(VisitedEventId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            var VisitedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.Get(VisitedEventId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task GetCheck_ReturnsVisitedEventDto_WhenVisitedEventExists()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;
            var VisitedEvent = new VisitedEvent { Id = 1, UserId = userId, EventId = eventId };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync(VisitedEvent);

            var result = await _controller.GetCheck(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(VisitedEventDto));
            var VisitedEventDto = result.Value;
            Assert.AreEqual(VisitedEvent.Id, VisitedEventDto.Id);
        }

        [TestMethod]
        public async Task GetCheck_ReturnsNotFound_WhenVisitedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync((VisitedEvent)null);

            var result = await _controller.GetCheck(eventId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetCheck_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            var eventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.GetCheck(eventId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Remove_ReturnsNoContent_WhenVisitedEventDeleted()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var VisitedEventId = 1;
            var VisitedEvent = new VisitedEvent { Id = VisitedEventId, UserId = userId };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, VisitedEventId)).ReturnsAsync(VisitedEvent);
            _VisitedEventRepositoryMock.Setup(repo => repo.DeleteAsync(VisitedEvent));

            var result = await _controller.Remove(VisitedEventId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFound_WhenVisitedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var VisitedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, VisitedEventId)).ReturnsAsync((VisitedEvent)null);

            var result = await _controller.Remove(VisitedEventId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            var VisitedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.Remove(VisitedEventId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task RemoveByEvent_ReturnsNoContent_WhenVisitedEventDeleted()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;
            var VisitedEvent = new VisitedEvent { Id = 1, UserId = userId, EventId = eventId };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync(VisitedEvent);
            _VisitedEventRepositoryMock.Setup(repo => repo.DeleteAsync(VisitedEvent));

            var result = await _controller.RemoveByEvent(eventId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task RemoveByEvent_ReturnsNotFound_WhenVisitedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _VisitedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync((VisitedEvent)null);

            var result = await _controller.RemoveByEvent(eventId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task RemoveByEvent_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            var eventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.RemoveByEvent(eventId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}