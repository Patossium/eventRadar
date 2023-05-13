using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eventRadar.Controllers;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Models;
using Microsoft.AspNetCore.Authorization;
using eventRadar.Auth.Model;
using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace eventRadar.Tests
{
    [TestClass]
    public class VisitedEventControllerTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEventRepository> _eventRepositoryMock;
        private Mock<IAuthorizationService> _authorizationServiceMock;
        private Mock<IVisitedEventRepository> _visitedEventRepositoryMock;
        private VisitedEventController _controller;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _authorizationServiceMock = new Mock<IAuthorizationService>();
            _visitedEventRepositoryMock = new Mock<IVisitedEventRepository>();

            _controller = new VisitedEventController(
                _userRepositoryMock.Object,
                _eventRepositoryMock.Object,
                _authorizationServiceMock.Object,
                _visitedEventRepositoryMock.Object
            );
        }

        [TestMethod]
        public async Task GetMany_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            string userId = "nonexistentUserId";
            int eventId = 1;
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetMany(userId, eventId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetMany_ReturnsNotFound_WhenEventNotFound()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            var user = new User { Id = userId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _controller.GetMany(userId, eventId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetMany_ReturnsOkWithVisitedEventDtoList()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            var user = new User { Id = userId };
            var eventObject = new Event { Id = eventId };
            var visitedEvents = new List<VisitedEvent>
            {
                new VisitedEvent { Id = 1, UserId = userId, User = user, EventId = eventId, Event = eventObject },
                new VisitedEvent { Id = 2, UserId = userId, User = user, EventId = eventId, Event = eventObject }
            };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(eventObject);
            _visitedEventRepositoryMock.Setup(repo => repo.GetManyAsync(user))
                .ReturnsAsync(visitedEvents);

            // Act
            var result = await _controller.GetMany(userId, eventId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<VisitedEventDto>));
            var visitedEventDtos = okResult.Value as IEnumerable<VisitedEventDto>;
            Assert.AreEqual(2, visitedEventDtos.Count());
        }
        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            string userId = "nonexistentUserId";
            int eventId = 1;
            int visitedEventId = 1;
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Get(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenEventNotFound()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            int visitedEventId = 1;
            var user = new User { Id = userId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync((Event)null);

            // Act
            var result = await _controller.Get(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenVisitedEventNotFound()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            int visitedEventId = 1;
            var user = new User { Id = userId };
            var eventObject = new Event { Id = eventId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(eventObject);
            _visitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, visitedEventId))
                .ReturnsAsync((VisitedEvent)null);

            // Act
            var result = await _controller.Get(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Get_ReturnsOkWithVisitedEventDto()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            int visitedEventId = 1;
            var user = new User { Id = userId };
            var eventObject = new Event { Id = eventId };
            var visitedEvent = new VisitedEvent { Id = visitedEventId, UserId = userId, User = user, EventId = eventId, Event = eventObject };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId))
                .ReturnsAsync(eventObject);
            _visitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, visitedEventId))
                .ReturnsAsync(visitedEvent);

            // Act
            var result = await _controller.Get(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<VisitedEventDto>));
            var okResult = result;
            Assert.IsNotNull(okResult.Value);
            Assert.IsInstanceOfType(okResult.Value, typeof(VisitedEventDto));
            var visitedEventDto = okResult.Value;
            Assert.AreEqual(visitedEvent.Id, visitedEventDto.Id);
            Assert.AreEqual(visitedEvent.UserId, visitedEventDto.userId);
            Assert.AreEqual(visitedEvent.User, visitedEventDto.User);
            Assert.AreEqual(visitedEvent.EventId, visitedEventDto.EventId);
            Assert.AreEqual(visitedEvent.Event, visitedEventDto.Event);
        }
        [TestMethod]
        public async Task Create_ValidData_ReturnsCreatedStatus()
        {
            // Arrange
            var userId = "testUserId";
            var eventId = 1;
            var createVisitedEventDto = new CreateVisitedEventDto();

            var user = new User { Id = userId };
            _userRepositoryMock.Setup(r => r.GetAsync(userId)).ReturnsAsync(user);

            var eventObject = new Event { Id = eventId };
            _eventRepositoryMock.Setup(r => r.GetAsync(eventId)).ReturnsAsync(eventObject);

            var visitedEvent = new VisitedEvent();
            _visitedEventRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<VisitedEvent>()))
                .Callback<VisitedEvent>(ve => visitedEvent = ve);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, userId) // Set the UserId claim
                    }))
                }
            };

            // Act
            var result = await _controller.Create(userId, eventId, createVisitedEventDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            Assert.IsNotNull(((CreatedResult)result.Result).Value);
            Assert.AreEqual(visitedEvent.Id, ((VisitedEventDto)((CreatedResult)result.Result).Value).Id);
            Assert.AreEqual(visitedEvent.UserId, ((VisitedEventDto)((CreatedResult)result.Result).Value).userId);
            Assert.AreEqual(visitedEvent.User, ((VisitedEventDto)((CreatedResult)result.Result).Value).User);
            Assert.AreEqual(visitedEvent.Event, ((VisitedEventDto)((CreatedResult)result.Result).Value).Event);
            Assert.AreEqual(visitedEvent.EventId, ((VisitedEventDto)((CreatedResult)result.Result).Value).EventId);
        }

        [TestMethod]
        public async Task Create_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            var userId = "testUserId";
            var eventId = 1;
            var createVisitedEventDto = new CreateVisitedEventDto();

            _userRepositoryMock.Setup(r => r.GetAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Create(userId, eventId, createVisitedEventDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_InvalidEvent_ReturnsNotFound()
        {
            // Arrange
            var userId = "testUserId";
            var eventId = 1;
            var createVisitedEventDto = new CreateVisitedEventDto();

            _userRepositoryMock.Setup(r => r.GetAsync(userId)).ReturnsAsync(new User());
            _eventRepositoryMock.Setup(r => r.GetAsync(eventId)).ReturnsAsync((Event)null);

            // Act
            var result = await _controller.Create(userId, eventId, createVisitedEventDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task GetCheck_VisitedEventExists_ReturnsFollowedEventDto()
        {
            // Arrange
            int eventId = 1;
            var userId = "testUserId";
            var user = new User { Id = userId };
            var visitedEvent = new VisitedEvent
            {
                Id = 1,
                UserId = userId,
                User = user,
                Event = new Event(),
                EventId = eventId
            };

            _userRepositoryMock.Setup(r => r.GetAsync(userId)).ReturnsAsync(user);
            _visitedEventRepositoryMock.Setup(r => r.GetCheckAsync(user, eventId)).ReturnsAsync(visitedEvent);

            // Set up the User identity for testing
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, userId)
                    }))
                }
            };

            // Act
            var result = await _controller.GetCheck(eventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<FollowedEventDto>));
            var actionResult = (ActionResult<FollowedEventDto>)result;
            Assert.IsNotNull(actionResult.Value);
            Assert.IsInstanceOfType(actionResult.Value, typeof(FollowedEventDto));
            var followedEventDto = actionResult.Value;
            Assert.AreEqual(visitedEvent.Id, followedEventDto.Id);
            Assert.AreEqual(visitedEvent.UserId, followedEventDto.UserId);
            Assert.AreEqual(visitedEvent.User, followedEventDto.User);
            Assert.AreEqual(visitedEvent.Event, followedEventDto.Event);
            Assert.AreEqual(visitedEvent.EventId, followedEventDto.EventId);
        }

        [TestMethod]
        public async Task GetCheck_VisitedEventDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            int eventId = 1;
            var userId = "testUserId";
            var user = new User { Id = userId };
            _userRepositoryMock.Setup(r => r.GetAsync(userId)).ReturnsAsync(user);
            _visitedEventRepositoryMock.Setup(r => r.GetCheckAsync(user, eventId)).ReturnsAsync((VisitedEvent)null);

            // Set up the User identity for testing
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                         new Claim(JwtRegisteredClaimNames.Sub, userId)
                    }))
                }
            };

            // Act
            var result = await _controller.GetCheck(eventId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Remove_ExistingVisitedEvent_ReturnsNoContent()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            int visitedEventId = 1;
            var user = new User { Id = userId };
            var eventObject = new Event { Id = eventId };
            var visitedEvent = new VisitedEvent { Id = visitedEventId, UserId = userId, User = user, EventId = eventId, Event = eventObject };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);
            _visitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, visitedEventId)).ReturnsAsync(visitedEvent);

            // Act
            var result = await _controller.Remove(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _visitedEventRepositoryMock.Verify(repo => repo.DeleteAsync(visitedEvent), Times.Once);
        }

        [TestMethod]
        public async Task Remove_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            string userId = "nonexistentUserId";
            int eventId = 1;
            int visitedEventId = 1;
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Remove(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Remove_EventNotFound_ReturnsNotFound()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            int visitedEventId = 1;
            var user = new User { Id = userId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync((Event)null);

            // Act
            var result = await _controller.Remove(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Remove_VisitedEventNotFound_ReturnsNotFound()
        {
            // Arrange
            string userId = "existingUserId";
            int eventId = 1;
            int visitedEventId = 1;
            var user = new User { Id = userId };
            var eventObject = new Event { Id = eventId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _eventRepositoryMock.Setup(repo => repo.GetAsync(eventId)).ReturnsAsync(eventObject);
            _visitedEventRepositoryMock.Setup(repo => repo.GetAsync(user, visitedEventId)).ReturnsAsync((VisitedEvent)null);

            // Act
            var result = await _controller.Remove(userId, eventId, visitedEventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task RemoveByEvent_ExistingFollowedEvent_ReturnsNoContent()
        {
            // Arrange
            int eventId = 1;
            string userId = "existingUserId";
            var user = new User { Id = userId };
            var followedEvent = new VisitedEvent { Id = 1, UserId = userId, User = user, EventId = eventId };

            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _visitedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync(followedEvent);

            // Set up the User identity for testing
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, userId)
                    }))
                }
            };

            // Act
            var result = await _controller.RemoveByEvent(eventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            _visitedEventRepositoryMock.Verify(repo => repo.DeleteAsync(followedEvent), Times.Once);
        }

        [TestMethod]
        public async Task RemoveByEvent_FollowedEventNotFound_ReturnsNotFound()
        {
            // Arrange
            int eventId = 1;
            string userId = "existingUserId";
            var user = new User { Id = userId };
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync(user);
            _visitedEventRepositoryMock.Setup(repo => repo.GetCheckAsync(user, eventId)).ReturnsAsync((VisitedEvent)null);

            // Set up the User identity for testing
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, userId)
                    }))
                }
            };

            // Act
            var result = await _controller.RemoveByEvent(eventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task RemoveByEvent_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            int eventId = 1;
            string userId = "nonexistentUserId";
            _userRepositoryMock.Setup(repo => repo.GetAsync(userId)).ReturnsAsync((User)null);

            // Set up the User identity for testing
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                new Claim(JwtRegisteredClaimNames.Sub, userId)
                    }))
                }
            };

            // Act
            var result = await _controller.RemoveByEvent(eventId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}