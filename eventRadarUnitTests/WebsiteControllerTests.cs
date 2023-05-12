using eventRadar.Controllers;
using eventRadar.Data.Dtos;
using eventRadar.Data.Repositories;
using eventRadar.Helpers;
using eventRadar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eventRadarUnitTests
{
    [TestClass]
    public class WebsiteControllerTests
    {
        private Mock<IEventRepository> _eventRepositoryMock;
        private EventController _controller;

        [TestInitialize]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _controller = new EventController(_eventRepositoryMock.Object, null, null);
        }
        private WebsiteController SetupControllerWithMockRepo(Mock<IWebsiteRepository> mockRepo)
        {
            return new WebsiteController(mockRepo.Object);
        }

        [TestMethod]
        public async Task GetMany_ReturnsListOfWebsites()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            var websites = new List<Website>
            {
                new Website { Id = 1, Url = "https://example1.com" },
                new Website { Id = 2, Url = "https://example2.com" }
            };
            mockRepo.Setup(repo => repo.GetManyAsync()).ReturnsAsync(websites);

            // Act
            var result = await controller.GetMany();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsNotFoundResult_WhenWebsiteDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            int nonExistingWebsiteId = 3;
            mockRepo.Setup(repo => repo.GetAsync(nonExistingWebsiteId)).ReturnsAsync((Website)null);

            // Act
            var result = await controller.Get(nonExistingWebsiteId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsOkObjectResult_WhenWebsiteExists()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            int existingWebsiteId = 1;
            var existingWebsite = new Website { Id = existingWebsiteId, Url = "https://example1.com" };
            mockRepo.Setup(repo => repo.GetAsync(existingWebsiteId)).ReturnsAsync(existingWebsite);

            // Act
            var result = await controller.Get(existingWebsiteId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<WebsiteDto>));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var websiteDto = okResult.Value as WebsiteDto;
            Assert.AreEqual(existingWebsite.Id, websiteDto.Id);
            Assert.AreEqual(existingWebsite.Url, websiteDto.Url);
        }
        [TestMethod]
        public async Task Create_ReturnsCreatedResult_WithWebsiteDto()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            var createWebsiteDto = new CreateWebsiteDto("https://example3.com", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath");
            var createdWebsite = new Website { Id = 3, Url = "https://example3.com" };
            mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Website>())).Returns(Task.CompletedTask).Callback<Website>(w => w.Id = 3);

            // Act
            var result = await controller.Create(createWebsiteDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdResult = result.Result as CreatedResult;
            Assert.IsNotNull(createdResult);
            var websiteDto = createdResult.Value as WebsiteDto;
            Assert.AreEqual(createdWebsite.Id, websiteDto.Id);
            Assert.AreEqual(createdWebsite.Url, websiteDto.Url);
        }
        [TestMethod]
        public async Task Create_CallsCreateAsyncOnRepository_Once()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            var createWebsiteDto = new CreateWebsiteDto("https://example4.com", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath", "testpath");
            mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Website>())).Returns(Task.CompletedTask).Callback<Website>(w => w.Id = 4);

            // Act
            var result = await controller.Create(createWebsiteDto);

            // Assert
            mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Website>()), Times.Once());
        }
        [TestMethod]
        public async Task Update_ReturnsNotFoundResult_WhenWebsiteDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            int websiteId = 1;
            var updateWebsiteDto = new UpdateWebsiteDto ("https://example5.com", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2");
            mockRepo.Setup(repo => repo.GetAsync(websiteId)).ReturnsAsync((Website)null);

            // Act
            var result = await controller.Update(websiteId, updateWebsiteDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOkObjectResult_WithUpdatedWebsiteDto()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            int websiteId = 1;
            var existingWebsite = new Website { Id = websiteId, Url = "https://example6.com" };
            var updateWebsiteDto = new UpdateWebsiteDto ("https://example7.com", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2", "testpath2");
            mockRepo.Setup(repo => repo.GetAsync(websiteId)).ReturnsAsync(existingWebsite);
            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Website>())).Returns(Task.CompletedTask);

            // Act
            var result = await controller.Update(websiteId, updateWebsiteDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var updatedWebsiteDto = okResult.Value as WebsiteDto;
            Assert.AreEqual(websiteId, updatedWebsiteDto.Id);
            Assert.AreEqual(updateWebsiteDto.Url, updatedWebsiteDto.Url);
        }
        [TestMethod]
        public async Task Remove_ReturnsNotFoundResult_WhenWebsiteDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            int websiteId = 1;
            mockRepo.Setup(repo => repo.GetAsync(websiteId)).ReturnsAsync((Website)null);

            // Act
            var result = await controller.Remove(websiteId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Remove_ReturnsNoContentResult_WhenWebsiteIsDeleted()
        {
            // Arrange
            var mockRepo = new Mock<IWebsiteRepository>();
            var controller = SetupControllerWithMockRepo(mockRepo);
            int websiteId = 1;
            var existingWebsite = new Website { Id = websiteId, Url = "https://example8.com" };
            mockRepo.Setup(repo => repo.GetAsync(websiteId)).ReturnsAsync(existingWebsite);
            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Website>())).Returns(Task.CompletedTask);

            // Act
            var result = await controller.Remove(websiteId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public async Task GetManyPaging_ReturnsPreviousPageLink_WhenPreviousPageExists()
        {
            // Arrange
            var searchParameters = new EventSearchParameters { PageNumber = 2, PageSize = 1 };
            var events = new List<Event> { new Event { Id = 1 } };
            _eventRepositoryMock.Setup(repo => repo.GetManyPagedAsync(searchParameters))
                .ReturnsAsync(new PagedList<Event>(events, events.Count, searchParameters.PageNumber, searchParameters.PageSize));

            // Act
            var result = await _controller.GetManyPaging(searchParameters);

            // Assert
            Assert.IsNotNull(_controller.Response.Headers["Pagination"]);
            var paginationHeader = _controller.Response.Headers["Pagination"].ToString();
            Assert.IsTrue(paginationHeader.Contains("\"previousPageLink\":"));
        }

        [TestMethod]
        public async Task GetManyFilteredPaging_ReturnsNextPageLink_WhenNextPageExists()
        {
            // Arrange
            var searchParameters = new EventSearchParameters { PageNumber = 1, PageSize = 1 };
            var events = new List<Event> { new Event { Id = 1 }, new Event { Id = 2 } };
            _eventRepositoryMock.Setup(repo => repo.GetManyFilteredAsync("TestCategory", searchParameters))
                .ReturnsAsync(new PagedList<Event>(events, events.Count, searchParameters.PageNumber, searchParameters.PageSize));

            // Act
            var result = await _controller.GetManyFilteredPaging(searchParameters, "TestCategory");

            // Assert
            Assert.IsNotNull(_controller.Response.Headers["Pagination"]);
            var paginationHeader = _controller.Response.Headers["Pagination"].ToString();
            Assert.IsTrue(paginationHeader.Contains("\"nextPageLink\":"));
        }
    }
}