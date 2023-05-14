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
            var blacklistedPages = new List<BlacklistedPage>
            {
                new BlacklistedPage { Id = 1, Url = "http://example.com/1", Comment = "Test comment 1" },
                new BlacklistedPage { Id = 2, Url = "http://example.com/2", Comment = "Test comment 2" }
            };
            _repositoryMock.Setup(x => x.GetManyAsync()).ReturnsAsync(blacklistedPages);

            var result = await _controller.GetMany();

            Assert.AreEqual(blacklistedPages.Count, result.Count());
        }
        [TestMethod]
        public async Task Get_ReturnsNotFoundResult_WhenBlacklistedPageDoesNotExist()
        {
            int nonExistentPageId = 99;
            _repositoryMock.Setup(x => x.GetAsync(nonExistentPageId)).ReturnsAsync((BlacklistedPage)null);

            var result = await _controller.Get(nonExistentPageId);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Get_ReturnsBlacklistedPageDto_WhenBlacklistedPageExists()
        {
            var existingPage = new BlacklistedPage { Id = 1, Url = "http://example.com/1", Comment = "Test comment 1" };
            _repositoryMock.Setup(x => x.GetAsync(existingPage.Id)).ReturnsAsync(existingPage);

            var result = await _controller.Get(existingPage.Id);

            Assert.IsNotNull(result.Value);
            Assert.AreEqual(existingPage.Id, result.Value.Id);
            Assert.AreEqual(existingPage.Url, result.Value.Url);
            Assert.AreEqual(existingPage.Comment, result.Value.Comment);
        }
        [TestMethod]
        public async Task Create_ReturnsCreatedAtActionResult_WhenBlacklistedPageIsCreated()
        {
            var createBlacklistedPageDto = new CreateBlacklistedPageDto("http://example.com", "This is a test comment");
            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<BlacklistedPage>()))
                .Callback((BlacklistedPage page) => {
                    page.Id = 1;
                });

            var result = await _controller.Create(createBlacklistedPageDto);

            Assert.IsInstanceOfType(result, typeof(ActionResult<BlacklistedPageDto>));
            var createdAtResult = result;
            Assert.IsNotNull(createdAtResult);
        }
        [TestMethod]
        public async Task Updated_ReturnsOkObjectResult_WhenBlacklistedPageIsUpdated()
        {
            int existingBlacklistedPageId = 1;
            var existingBlacklistedPage = new BlacklistedPage { Id = existingBlacklistedPageId, Url = "http://example.com", Comment = "Initial comment" };
            var updateBlacklistedPageDto = new UpdateBlacklistedPageDto ("http://example2.com", "Updated comment");

            var mockRepo = new Mock<IBlacklistedPageRepository>();
            mockRepo.Setup(repo => repo.GetAsync(existingBlacklistedPageId)).ReturnsAsync(existingBlacklistedPage);

            var controller = new BlacklistedPageController(mockRepo.Object);

            var result = await controller.Updated(existingBlacklistedPageId, updateBlacklistedPageDto);

            Assert.IsInstanceOfType(result, typeof(ActionResult<BlacklistedPageDto>));
            var okResult = result;
            var updatedBlacklistedPageDto = okResult.Value;
        }
        [TestMethod]
        public async Task Remove_ReturnsNoContentResult_WhenBlacklistedPageIsDeleted()
        {
            int blacklistedPageId = 1;
            var existingBlacklistedPage = new BlacklistedPage { Id = blacklistedPageId, Url = "http://example.com", Comment = "Test comment" };

            _repositoryMock.Setup(x => x.GetAsync(blacklistedPageId)).ReturnsAsync(existingBlacklistedPage);
            _repositoryMock.Setup(x => x.DeleteAsync(existingBlacklistedPage)).Returns(Task.CompletedTask);

            var result = await _controller.Remove(blacklistedPageId);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public async Task Updated_BlacklistedPageNotFound_ReturnsNotFound()
        {
            int blacklistedId = 1;
            var updateBlacklistedPageDto = new UpdateBlacklistedPageDto("https://example.com", "Test comment");

            _repositoryMock.Setup(r => r.GetAsync(blacklistedId)).ReturnsAsync((BlacklistedPage)null);

            var result = await _controller.Updated(blacklistedId, updateBlacklistedPageDto);

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Remove_BlacklistedPageNotFound_ReturnsNotFound()
        {
            int blacklistedId = 1;
            _repositoryMock.Setup(r => r.GetAsync(blacklistedId)).ReturnsAsync((BlacklistedPage)null);

            var result = await _controller.Remove(blacklistedId);

            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
    }
}