using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eventRadar.Controllers;
using eventRadar.Data.Repositories;
using eventRadar.Models;
using eventRadar.Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace eventRadar.Tests
{
    [TestClass]
    public class LocationControllerTests
    {
        private Mock<ILocationRepository> _locationRepositoryMock;
        private LocationController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _controller = new LocationController(_locationRepositoryMock.Object);
        }
        [TestMethod]
        public async Task GetMany_ReturnsListOfLocationDtos()
        {
            // Arrange
            var locations = new List<Location>
        {
            new Location { Id = 1, Name = "Location 1", City = "City 1", Country = "Country 1", Address = "Address 1" },
            new Location { Id = 2, Name = "Location 2", City = "City 2", Country = "Country 2", Address = "Address 2" }
        };

            _locationRepositoryMock.Setup(repo => repo.GetManyAsync()).ReturnsAsync(locations);

            // Act
            var result = await _controller.GetMany();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task Get_ReturnsLocationDto_WhenLocationExists()
        {
            // Arrange
            var locationId = 1;
            var location = new Location { Id = locationId, Name = "Location 1", City = "City 1", Country = "Country 1", Address = "Address 1" };

            _locationRepositoryMock.Setup(repo => repo.GetAsync(locationId)).ReturnsAsync(location);

            // Act
            var result = await _controller.Get(locationId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(LocationDto));
            Assert.AreEqual(locationId, result.Value.Id);
        }
        [TestMethod]
        public async Task Get_ReturnsNotFound_WhenLocationDoesNotExist()
        {
            // Arrange
            var locationId = 1;
            _locationRepositoryMock.Setup(repo => repo.GetAsync(locationId)).ReturnsAsync((Location)null);

            // Act
            var result = await _controller.Get(locationId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Create_ReturnsCreatedLocationDto()
        {
            // Arrange
            var createLocationDto = new CreateLocationDto ("Location 1","City 1", "Country 1",  "Address 1");

            _locationRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Location>())).Callback<Location>(l => l.Id = 1);

            // Act
            var result = await _controller.Create(createLocationDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(CreatedResult));
            Assert.IsInstanceOfType(((CreatedResult)result.Result).Value, typeof(LocationDto));
            Assert.AreEqual(1, ((LocationDto)((CreatedResult)result.Result).Value).Id);
        }

        [TestMethod]
        public async Task Update_ReturnsOk_WhenLocationIsUpdated()
        {
            // Arrange
            int locationId = 1;
            var location = new Location
            {
                Id = locationId,
                Name = "Original Name",
                City = "Original City",
                Country = "Original Country",
                Address = "Original Address"
            };

            var updateLocationDto = new UpdateLocationDto("Updated Name", "Updated City", "Updated Country", "Updated Address");

            _locationRepositoryMock.Setup(repo => repo.GetAsync(locationId))
                .ReturnsAsync(location);
            _locationRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Location>()));

            // Act
            var result = await _controller.Update(locationId, updateLocationDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ActionResult<LocationDto>));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(((OkObjectResult)result.Result).Value, typeof(LocationDto));
            var updatedLocationDto = (LocationDto)((OkObjectResult)result.Result).Value;
            Assert.AreEqual(locationId, updatedLocationDto.Id);
            Assert.AreEqual(updateLocationDto.Name, updatedLocationDto.Name);
            Assert.AreEqual(updateLocationDto.City, updatedLocationDto.City);
            Assert.AreEqual(updateLocationDto.Country, updatedLocationDto.Country);
            Assert.AreEqual(updateLocationDto.Address, updatedLocationDto.Address);
        }
        [TestMethod]
        public async Task Remove_ReturnsNotFound_WhenLocationNotFound()
        {
            // Arrange
            int locationId = 1;
            _locationRepositoryMock.Setup(repo => repo.GetAsync(locationId))
                .ReturnsAsync((Location)null);

            // Act
            var result = await _controller.Remove(locationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Remove_ReturnsNoContent_WhenLocationRemoved()
        {
            // Arrange
            int locationId = 1;
            var location = new Location
            {
                Id = locationId,
                Name = "Test Location",
                City = "Test City",
                Country = "Test Country",
                Address = "Test Address"
            };

            _locationRepositoryMock.Setup(repo => repo.GetAsync(locationId))
                .ReturnsAsync(location);
            _locationRepositoryMock.Setup(repo => repo.DeleteAsync(location))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Remove(locationId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }
        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenLocationNotFound()
        {
            // Arrange
            int locationId = 1;
            var updateLocationDto = new UpdateLocationDto("Updated Name", "Updated City", "Updated Country", "Updated Address");

            _locationRepositoryMock.Setup(repo => repo.GetAsync(locationId))
                .ReturnsAsync((Location)null);

            // Act
            var result = await _controller.Update(locationId, updateLocationDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ActionResult<LocationDto>));
            var actionResult = result as ActionResult<LocationDto>;
            Assert.IsNotNull(actionResult.Result);
            Assert.IsInstanceOfType(actionResult.Result, typeof(NotFoundResult));
        }
    }
}
