using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using eventRadar.Controllers;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Auth.Model;
using eventRadar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eventRadar.Tests
{
    [TestClass]
    public class BlacklistedCategoryNameControllerTests
    {
        private BlacklistedCategoryNameController _controller;
        private Mock<IBlacklistedCategoryNameRepository> _repositoryMock;
        private ClaimsPrincipal _administratorPrincipal;

        [TestInitialize]
        public void TestInitialize()
        {
            _repositoryMock = new Mock<IBlacklistedCategoryNameRepository>();
            _controller = new BlacklistedCategoryNameController(_repositoryMock.Object);

            var claims = new List<Claim> { new Claim(ClaimTypes.Role, SystemRoles.Administrator) };
            var identity = new ClaimsIdentity(claims);
            _administratorPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = _administratorPrincipal } };
        }

        [TestMethod]
        public async Task GetMany_ReturnsListOfBlacklistedCategoryNameDtos()
        {
            // Arrange
            var blacklistedCategories = new List<BlacklistedCategoryName>
            {
                new BlacklistedCategoryName { Id = 1, Name = "Category1" },
                new BlacklistedCategoryName { Id = 2, Name = "Category2" }
            };
            _repositoryMock.Setup(x => x.GetManyAsync()).ReturnsAsync(blacklistedCategories);

            // Act
            var result = await _controller.GetMany();

            // Assert
            Assert.AreEqual(2, result.Count());
            CollectionAssert.AllItemsAreInstancesOfType(result.ToList(), typeof(BlacklistedCategoryNameDto));
        }
        [TestMethod]
        public async Task Get_ReturnsNotFoundResult_WhenBlacklistedCategoryNameDoesNotExist()
        {
            // Arrange
            int nonExistentCategoryId = 99;
            _repositoryMock.Setup(x => x.GetAsync(nonExistentCategoryId)).ReturnsAsync((BlacklistedCategoryName)null);

            // Act
            var result = await _controller.Get(nonExistentCategoryId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Get_ReturnsBlacklistedCategoryNameDto_WhenBlacklistedCategoryNameExists()
        {
            // Arrange
            var blacklistedCategory = new BlacklistedCategoryName { Id = 1, Name = "Category1" };
            _repositoryMock.Setup(x => x.GetAsync(blacklistedCategory.Id)).ReturnsAsync(blacklistedCategory);

            // Act
            var result = await _controller.Get(blacklistedCategory.Id);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(blacklistedCategory.Id, result.Value.Id);
            Assert.AreEqual(blacklistedCategory.Name, result.Value.Name);
        }
        [TestMethod]
        public async Task Create_ReturnsCreatedAtActionResult_WhenBlacklistedCategoryNameIsCreated()
        {
            // Arrange
            var createDto = new CreateBlacklistedCategoryNameDto ("Category1");
            var createdCategory = new BlacklistedCategoryName { Id = 1, Name = createDto.Name };
            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<BlacklistedCategoryName>())).Callback<BlacklistedCategoryName>(c => c.Id = 1);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdAtResult = result.Result as CreatedResult;
            var createdDto = createdAtResult.Value as BlacklistedCategoryNameDto;
            Assert.AreEqual(createdCategory.Id, createdDto.Id);
            Assert.AreEqual(createdCategory.Name, createdDto.Name);
        }
        [TestMethod]
        public async Task Updated_ReturnsNotFoundResult_WhenBlacklistedCategoryNameDoesNotExist()
        {
            // Arrange
            int nonExistentCategoryId = 99;
            var updateDto = new UpdateBlacklistedCategoryNameDto("Category1");
            _repositoryMock.Setup(x => x.GetAsync(nonExistentCategoryId)).ReturnsAsync((BlacklistedCategoryName)null);

            // Act
            var result = await _controller.Updated(nonExistentCategoryId, updateDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Updated_ReturnsOkObjectResult_WhenBlacklistedCategoryNameIsUpdated()
        {
            // Arrange
            var existingCategory = new BlacklistedCategoryName { Id = 1, Name = "Category1" };
            var updateDto = new UpdateBlacklistedCategoryNameDto("Category1");
            _repositoryMock.Setup(x => x.GetAsync(existingCategory.Id)).ReturnsAsync(existingCategory);

            // Act
            var result = await _controller.Updated(existingCategory.Id, updateDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var updatedDto = okResult.Value as BlacklistedCategoryNameDto;
            Assert.AreEqual(existingCategory.Id, updatedDto.Id);
            Assert.AreEqual(updateDto.Name, updatedDto.Name);
        }
        [TestMethod]
        public async Task Remove_ReturnsNotFoundResult_WhenBlacklistedCategoryNameDoesNotExist()
        {
            // Arrange
            int nonExistentCategoryId = 99;
            _repositoryMock.Setup(x => x.GetAsync(nonExistentCategoryId)).ReturnsAsync((BlacklistedCategoryName)null);

            // Act
            var result = await _controller.Remove(nonExistentCategoryId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Remove_ReturnsNoContentResult_WhenBlacklistedCategoryNameIsDeleted()
        {
            // Arrange
            var existingCategory = new BlacklistedCategoryName { Id = 1, Name = "Category1" };
            _repositoryMock.Setup(x => x.GetAsync(existingCategory.Id)).ReturnsAsync(existingCategory);

            // Act
            var result = await _controller.Remove(existingCategory.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}