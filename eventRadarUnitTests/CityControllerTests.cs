using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using eventRadar.Controllers;
using eventRadar.Data.Repositories;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace eventRadarUnitTests
{
    [TestClass]
    public class CityControllerTests
    {
        [TestMethod]
        public async Task GetMany_ReturnsListOfCityDtos()
        {
            // Arrange
            var mockRepo = new Mock<ICityRepository>();
            mockRepo.Setup(repo => repo.GetManyAsync()).ReturnsAsync(new List<City>
            {
                new City { Id = 1, Name = "City 1" },
                new City { Id = 2, Name = "City 2" }
            });
            var controller = new CityController(mockRepo.Object);

            // Act
            var result = await controller.GetMany();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("City 1", result.ElementAt(0).Name);
            Assert.AreEqual("City 2", result.ElementAt(1).Name);
        }

        [TestMethod]
        public async Task Get_ReturnsNotFoundResult_WhenCityDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<ICityRepository>();
            mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((City)null);
            var controller = new CityController(mockRepo.Object);

            // Act
            var result = await controller.Get(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Get_ReturnsCityDto_WhenCityExists()
        {
            // Arrange
            var mockRepo = new Mock<ICityRepository>();
            mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(new City { Id = 1, Name = "City 1" });
            var controller = new CityController(mockRepo.Object);

            // Act
            var result = await controller.Get(1);

            // Assert
            Assert.IsInstanceOfType(result.Value, typeof(CityDto));
            Assert.AreEqual(1, result.Value.Id);
            Assert.AreEqual("City 1", result.Value.Name);
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedResult_WhenCityIsCreated()
        {
            // Arrange
            var mockRepo = new Mock<ICityRepository>();
            mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<City>())).Returns(Task.CompletedTask);
            var controller = new CityController(mockRepo.Object);
            var createCityDto = new CreateCityDto ("City 1");

            // Act
            var result = await controller.Create(createCityDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            var createdResult = result.Result as CreatedResult;
            var cityDto = createdResult.Value as CityDto;
            Assert.IsNotNull(cityDto);
            Assert.AreEqual("City 1", cityDto.Name);
        }

        [TestMethod]
        public async Task Remove_ReturnsNotFoundResult_WhenCityDoesNotExist()
        {
            // Arrange
            var mockRepo = new Mock<ICityRepository>();
            mockRepo.Setup(repo => repo.GetAsync(It.IsAny<int>())).ReturnsAsync((City)null);
            var controller = new CityController(mockRepo.Object);

            // Act
            var result = await controller.Remove(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }
        [TestMethod]
        public async Task Remove_ReturnsNoContentResult_WhenCityIsDeleted()
        {
            // Arrange
            var mockRepo = new Mock<ICityRepository>();
            mockRepo.Setup(repo => repo.GetAsync(1)).ReturnsAsync(new City { Id = 1, Name = "City 1" });
            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<City>())).Returns(Task.CompletedTask);
            var controller = new CityController(mockRepo.Object);

            // Act
            var result = await controller.Remove(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
    }
}
