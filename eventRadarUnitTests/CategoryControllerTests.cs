using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eventRadar.Controllers;
using eventRadar.Data.Repositories;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace eventRadarUnitTests
{
    [TestClass]
    public class CategoryControllerTests
    {
        private Mock<ICategoryRepository> _categoryRepositoryMock;
        private CategoryController _controller;

        [TestInitialize]
        public void Initialize()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _controller = new CategoryController(_categoryRepositoryMock.Object);
        }
        private CategoryController SetupControllerWithMockRepo(Mock<ICategoryRepository> mockRepo)
        {
            return new CategoryController(mockRepo.Object);
        }

        [TestMethod]
        public async Task GetMany_ReturnsAllCategories()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Category 1", SourceUrl = "https://example1.com" },
                new Category { Id = 2, Name = "Category 2", SourceUrl = "https://example2.com" },
            };
            mockRepo.Setup(repo => repo.GetManyAsync()).ReturnsAsync(categories);
            var controller = SetupControllerWithMockRepo(mockRepo);

            // Act
            var result = await controller.GetMany();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
        [TestMethod]
        public async Task Get_ExistingCategoryId_ReturnsCategoryDto()
        {
            // Arrange
            int categoryId = 1;
            var category = new Category { Id = categoryId, Name = "Category 1", SourceUrl = "https://example.com/category1" };
            _categoryRepositoryMock.Setup(r => r.GetAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _controller.Get(categoryId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(CategoryDto));
            var categoryDto = (CategoryDto)result.Value;
            Assert.AreEqual(category.Id, categoryDto.Id);
            Assert.AreEqual(category.Name, categoryDto.Name);
            Assert.AreEqual(category.SourceUrl, categoryDto.SourceUrl);
        }
        [TestMethod]
        public async Task Get_ReturnsNotFoundResult_WhenCategoryDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((Category)null);
            var controller = SetupControllerWithMockRepo(mockRepo);

            // Act
            var result = await controller.Get(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Update_ReturnsNotFoundResult_WhenCategoryDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((Category)null);
            var controller = new CategoryController(mockRepo.Object);
            var updateCategoryDto = new UpdateCategoryDto ("Category 1", "https://newurl.com" );

            // Act
            var result = await controller.Update(1, updateCategoryDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Update_ReturnsOkObjectResult_WhenCategoryIsUpdated()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var category = new Category { Id = 1, Name = "Category 1", SourceUrl = "https://example1.com" };
            mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(category);
            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            var controller = new CategoryController(mockRepo.Object);
            var updateCategoryDto = new UpdateCategoryDto ("Category 1 - Updated", "http://newUrl.com");

            // Act
            var result = await controller.Update(1, updateCategoryDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var categoryDto = okResult.Value as CategoryDto;
            Assert.IsNotNull(categoryDto);
            Assert.AreEqual(1, categoryDto.Id);
            Assert.AreEqual("Category 1 - Updated", categoryDto.Name);
            Assert.AreEqual("https://example1.com", categoryDto.SourceUrl);
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFoundResult_WhenCategoryDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((Category)null);
            var controller = new CategoryController(mockRepo.Object);

            // Act
            var result = await controller.Remove(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Remove_ReturnsNoContentResult_WhenCategoryIsDeleted()
        {
            // Arrange
            var mockRepo = new Mock<ICategoryRepository>();
            var category = new Category { Id = 1, Name = "Category 1", SourceUrl = "https://example1.com" };
            mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(category);
            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            var controller = new CategoryController(mockRepo.Object);

            // Act
            var result = await controller.Remove(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public async Task Create_ValidData_ReturnsCreatedStatus()
        {
            // Arrange
            var createCategoryDto = new CreateCategoryDto("Category 1", "https://example.com/category1");

            var category = new Category { Id = 1, Name = createCategoryDto.Name, SourceUrl = createCategoryDto.SourceUrl };
            _categoryRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Category>()))
                .Callback<Category>(c => category = c);

            // Act
            var result = await _controller.Create(createCategoryDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdResult = (CreatedResult)result.Result;
            Assert.IsNotNull(createdResult.Value);
            Assert.IsInstanceOfType(createdResult.Value, typeof(CategoryDto));
            var categoryDto = (CategoryDto)createdResult.Value;
            Assert.AreEqual(category.Id, categoryDto.Id);
            Assert.AreEqual(category.Name, categoryDto.Name);
            Assert.AreEqual(category.SourceUrl, categoryDto.SourceUrl);
        }
    }
}