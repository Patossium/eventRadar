using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using eventRadar.Controllers;
using eventRadar.Data.Repositories;
using eventRadar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eventRadar.Data.Dtos;
using eventRadar.Auth.Model;
using Microsoft.AspNetCore.Http;

namespace eventRadarUnitTests
{
    [TestClass]
    public class BlacklistedPageControllerTests
    {
        private BlacklistedPageController _controller;
        private Mock<IBlacklistedPageRepository> _repositoryMock;

        [TestInitialize]
        public void TestInitialize()
        {
            _repositoryMock = new Mock<IBlacklistedPageRepository>();
            _controller = new BlacklistedPageController(_repositoryMock.Object);
        }

        [TestMethod]
        public async Task GetMany_ReturnsBlacklistedPageDtos()
        {
            // Arrange
            var blacklistedPages = new List<BlacklistedPage>
            {
                new BlacklistedPage { Id = 1, Url = "http://example.com/1", Comment = "Test comment 1" },
                new BlacklistedPage { Id = 2, Url = "http://example.com/2", Comment = "Test comment 2" }
            };
            _repositoryMock.Setup(x => x.GetManyAsync()).ReturnsAsync(blacklistedPages);

            // Act
            var result = await _controller.GetMany();

            // Assert
            Assert.AreEqual(blacklistedPages.Count, result.Count());
        }
        [TestMethod]
        public async Task Get_ReturnsNotFoundResult_WhenBlacklistedPageDoesNotExist()
        {
            // Arrange
            int nonExistentPageId = 99;
            _repositoryMock.Setup(x => x.GetAsync(nonExistentPageId)).ReturnsAsync((BlacklistedPage)null);

            // Act
            var result = await _controller.Get(nonExistentPageId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Get_ReturnsBlacklistedPageDto_WhenBlacklistedPageExists()
        {
            // Arrange
            var existingPage = new BlacklistedPage { Id = 1, Url = "http://example.com/1", Comment = "Test comment 1" };
            _repositoryMock.Setup(x => x.GetAsync(existingPage.Id)).ReturnsAsync(existingPage);

            // Act
            var result = await _controller.Get(existingPage.Id);

            // Assert
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(existingPage.Id, result.Value.Id);
            Assert.AreEqual(existingPage.Url, result.Value.Url);
            Assert.AreEqual(existingPage.Comment, result.Value.Comment);
        }
        [TestMethod]
        public async Task Create_ReturnsCreatedAtActionResult_WhenBlacklistedPageIsCreated()
        {
            // Arrange
            var createBlacklistedPageDto = new CreateBlacklistedPageDto("http://example.com", "This is a test comment");
            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<BlacklistedPage>()))
                .Callback((BlacklistedPage page) => {
                    page.Id = 1;
                });

            // Act
            var result = await _controller.Create(createBlacklistedPageDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<BlacklistedPageDto>));
            var createdAtResult = result;
            Assert.IsNotNull(createdAtResult);
        }
        [TestMethod]
        public async Task Updated_ReturnsOkObjectResult_WhenBlacklistedPageIsUpdated()
        {
            // Arrange
            int existingBlacklistedPageId = 1;
            var existingBlacklistedPage = new BlacklistedPage { Id = existingBlacklistedPageId, Url = "http://example.com", Comment = "Initial comment" };
            var updateBlacklistedPageDto = new UpdateBlacklistedPageDto ("http://example2.com", "Updated comment");

            var mockRepo = new Mock<IBlacklistedPageRepository>();
            mockRepo.Setup(repo => repo.GetAsync(existingBlacklistedPageId)).ReturnsAsync(existingBlacklistedPage);

            var controller = new BlacklistedPageController(mockRepo.Object);

            // Act
            var result = await controller.Updated(existingBlacklistedPageId, updateBlacklistedPageDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<BlacklistedPageDto>));
            var okResult = result;
            var updatedBlacklistedPageDto = okResult.Value;
        }
        [TestMethod]
        public async Task Remove_ReturnsNoContentResult_WhenBlacklistedPageIsDeleted()
        {
            // Arrange
            int blacklistedPageId = 1;
            var existingBlacklistedPage = new BlacklistedPage { Id = blacklistedPageId, Url = "http://example.com", Comment = "Test comment" };

            _repositoryMock.Setup(x => x.GetAsync(blacklistedPageId)).ReturnsAsync(existingBlacklistedPage);
            _repositoryMock.Setup(x => x.DeleteAsync(existingBlacklistedPage)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Remove(blacklistedPageId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public async Task Updated_BlacklistedPageNotFound_ReturnsNotFound()
        {
            // Arrange
            int blacklistedId = 1;
            var updateBlacklistedPageDto = new UpdateBlacklistedPageDto("https://example.com", "Test comment");

            _repositoryMock.Setup(r => r.GetAsync(blacklistedId)).ReturnsAsync((BlacklistedPage)null);

            // Act
            var result = await _controller.Updated(blacklistedId, updateBlacklistedPageDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Remove_BlacklistedPageNotFound_ReturnsNotFound()
        {
            // Arrange
            int blacklistedId = 1;
            _repositoryMock.Setup(r => r.GetAsync(blacklistedId)).ReturnsAsync((BlacklistedPage)null);

            // Act
            var result = await _controller.Remove(blacklistedId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}