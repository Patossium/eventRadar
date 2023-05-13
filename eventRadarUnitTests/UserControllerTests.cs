using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using eventRadar.Controllers;
using eventRadar.Data.Repositories;
using eventRadar.Data.Dtos;
using eventRadar.Models;
using eventRadar.Auth.Model;

namespace eventRadar.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private UserController SetupControllerWithMockRepo(Mock<IUserRepository> mockRepo)
        {
            var controller = new UserController(mockRepo.Object);
            return controller;
        }

        [TestMethod]
        public async Task GetMany_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            var emptyUserList = new List<User>();
            mockRepo.Setup(repo => repo.GetManyAsync()).ReturnsAsync(emptyUserList);

            // Act
            var result = await controller.GetMany();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task GetMany_ReturnsUserDtoList_WhenUsersExist()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            var userList = new List<User>
            {
                new User { Id = "1", UserName = "user1", Email = "user1@example.com" },
                new User { Id = "2", UserName = "user2", Email = "user2@example.com" },
            };
            mockRepo.Setup(repo => repo.GetManyAsync()).ReturnsAsync(userList);

            // Act
            var result = await controller.GetMany();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userList.Count, result.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFoundResult_WhenUserDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            string nonExistentUserId = "1";

            // Act
            var result = await controller.Get(nonExistentUserId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOkObjectResult_WhenUserExists()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            string existingUserId = "1";
            var existingUser = new User { Id = existingUserId, UserName = "user1", Email = "user1@example.com" };

            mockRepo.Setup(repo => repo.GetAsync(existingUserId)).ReturnsAsync(existingUser);

            // Act
            var result = await controller.Get(existingUserId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<UserDto>));
            var okResult = result;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOfType(okResult.Value, typeof(UserDto));
            var userDto = okResult.Value as UserDto;
            Assert.AreEqual(existingUser.Id, userDto.Id);
            Assert.AreEqual(existingUser.UserName, userDto.Username);
            Assert.AreEqual(existingUser.Email, userDto.Email);
        }
    }
}