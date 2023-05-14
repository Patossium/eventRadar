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
    public class FollowedEventControllerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IAuthorizationService> _authorizationServiceMock;
        private readonly Mock<IFollowedEventRepository> _followedEventRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly FollowedEventController _controller;

        public FollowedEventControllerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _followedEventRepositoryMock = new Mock<IFollowedEventRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _controller = new FollowedEventController(
                _eventRepositoryMock.Object,
                _authorizationServiceMock.Object,
                _followedEventRepositoryMock.Object,
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
        public async Task Create_ReturnsCreatedAtActionResult_WhenFollowedEventIsCreated()
        {
            int eventId = 1;
            var user = new User { Id = "testuserid" };
            var eventObject = new Event { Id = eventId };
            var followedEvent = new FollowedEvent { Id = 1, UserId = "testuserid", User = user, EventId = eventId, Event = eventObject };

            _userRepositoryMock.Setup(repo => repo.GetAsync("testuserid")).ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);
            _followedEventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<FollowedEvent>()))
                .Callback((FollowedEvent fe) =>
                {
                    fe.Id = 1;
                });

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdResult = result.Result as CreatedResult;
            Assert.IsNotNull(createdResult.Value);
            Assert.IsInstanceOfType(createdResult.Value, typeof(FollowedEventDto));
            var followedEventDto = createdResult.Value as FollowedEventDto;
            Assert.AreEqual(followedEvent.Id, followedEventDto.Id);
            Assert.AreEqual(followedEvent.UserId, followedEventDto.UserId);
            Assert.AreEqual(followedEvent.User, followedEventDto.User);
            Assert.AreEqual(followedEvent.EventId, followedEventDto.EventId);
            Assert.AreEqual(followedEvent.Event, followedEventDto.Event);
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
        public async Task Create_CallsFollowedEventRepositoryCreateAsync()
        {
            var eventId = 1;
            var user = new User { Id = "testuserid" };
            var eventObject = new Event { Id = eventId };
            _userRepositoryMock.Setup(repo => repo.GetAsync("testuserid"))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(eventObject);
            var followedEvent = new FollowedEvent();
            _followedEventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<FollowedEvent>()))
                .Callback((FollowedEvent fe) => followedEvent = fe)
                .Returns(Task.CompletedTask);

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdResult = result.Result as CreatedResult;
            Assert.IsNotNull(createdResult.Value);
            Assert.IsInstanceOfType(createdResult.Value, typeof(FollowedEventDto));
            var followedEventDto = createdResult.Value as FollowedEventDto;
            Assert.AreEqual(followedEvent.Id, followedEventDto.Id);
            Assert.AreEqual(followedEvent.UserId, followedEventDto.UserId);
            Assert.AreEqual(followedEvent.User, followedEventDto.User);
            Assert.AreEqual(followedEvent.Event, followedEventDto.Event);
            Assert.AreEqual(followedEvent.EventId, followedEventDto.EventId);
            _followedEventRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<FollowedEvent>()), Times.Once());
        }
        [TestMethod]
        public async Task Create_ReturnsInternalServerError_WhenFollowedEventCreationFails()
        {
            var userId = "existingUserId";
            var eventId = 1;
            var user = new User { Id = userId };
            var eventObject = new Event { Id = eventId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(eventObject);
            _followedEventRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<FollowedEvent>()))
                .ThrowsAsync(new Exception("Failed to create followed event"));

            var result = await _controller.Create(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<FollowedEventDto>));
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
        public async Task GetMany_ReturnsOkObjectResult_WhenUserHasFollowedEvents()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var followedEvents = new List<FollowedEvent>
            {
                new FollowedEvent { Id = 1, UserId = userId, EventId = 1 },
                new FollowedEvent { Id = 2, UserId = userId, EventId = 2 }
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetManyAsync(user)).ReturnsAsync(followedEvents);

            var result = await _controller.GetMany();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var followedEventsDto = okResult.Value as IEnumerable<FollowedEventDto>;
            Assert.AreEqual(2, followedEventsDto.Count());
        }

        [TestMethod]
        public async Task GetMany_ReturnsOkObjectResult_WhenUserHasNoFollowedEvents()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var followedEvents = new List<FollowedEvent>();

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetManyAsync(user)).ReturnsAsync(followedEvents);

            var result = await _controller.GetMany();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var followedEventsDto = okResult.Value as IEnumerable<FollowedEventDto>;
            Assert.AreEqual(0, followedEventsDto.Count());
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
        public async Task Get_ReturnsFollowedEventDto_WhenFollowedEventExists()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var followedEventId = 1;
            var followedEvent = new FollowedEvent { Id = followedEventId, UserId = userId, EventId = 1 };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetAsync(user, followedEventId)).ReturnsAsync(followedEvent);

            var result = await _controller.Get(followedEventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(FollowedEventDto));
            var followedEventDto = result.Value;
            Assert.AreEqual(followedEvent.Id, followedEventDto.Id);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenFollowedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var followedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetAsync(user, followedEventId)).ReturnsAsync((FollowedEvent)null);

            var result = await _controller.Get(followedEventId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            var followedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.Get(followedEventId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task GetCheck_ReturnsFollowedEventDto_WhenFollowedEventExists()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;
            var followedEvent = new FollowedEvent { Id = 1, UserId = userId, EventId = eventId };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync(followedEvent);

            var result = await _controller.GetCheck(eventId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(FollowedEventDto));
            var followedEventDto = result.Value;
            Assert.AreEqual(followedEvent.Id, followedEventDto.Id);
        }

        [TestMethod]
        public async Task GetCheck_ReturnsNotFound_WhenFollowedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync((FollowedEvent)null);

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
        public async Task Remove_ReturnsNoContent_WhenFollowedEventDeleted()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var followedEventId = 1;
            var followedEvent = new FollowedEvent { Id = followedEventId, UserId = userId };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetAsync(user, followedEventId)).ReturnsAsync(followedEvent);
            _followedEventRepositoryMock.Setup(repo => repo.DeleteAsync(followedEvent));

            var result = await _controller.Remove(followedEventId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFound_WhenFollowedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var followedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetAsync(user, followedEventId)).ReturnsAsync((FollowedEvent)null);

            var result = await _controller.Remove(followedEventId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFound_WhenUserNotFound()
        {
            var userId = "nonexistentUserId";
            var followedEventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            var result = await _controller.Remove(followedEventId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task RemoveByEvent_ReturnsNoContent_WhenFollowedEventDeleted()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;
            var followedEvent = new FollowedEvent { Id = 1, UserId = userId, EventId = eventId };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync(followedEvent);
            _followedEventRepositoryMock.Setup(repo => repo.DeleteAsync(followedEvent));

            var result = await _controller.RemoveByEvent(eventId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task RemoveByEvent_ReturnsNotFound_WhenFollowedEventNotFound()
        {
            var userId = "testuserid";
            var user = new User { Id = userId };
            var eventId = 1;

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _followedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync((FollowedEvent)null);

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