using Doghouse.Controllers;
using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doghouse.Tests.ControllerTests
{
    public class DogControllerTests
    {
        private readonly DogController _dogController;
        private readonly Mock<IDogRepository> _mockDogRepository;
        public DogControllerTests()
        {
            _mockDogRepository = new Mock<IDogRepository>();
            _dogController = new DogController(_mockDogRepository.Object);
        }

        [Fact]
        public async Task GetAllDogsAsync_ValidQuery_ReturnsOkResultWithDogs()
        {
            // Arrange
            var query = new DogQueryObject { PageNumber = 1, PageSize = 2, Attribute = "name", Order = "asc" };
            var dogs = new PaginatedList<Dog>(new List<Dog> { new Dog { Name = "PesPatron" } }, 1, 1, 2);

            _mockDogRepository.Setup(repo => repo.GetAllAsync(query)).ReturnsAsync(dogs);

            // Act
            var result = await _dogController.GetAllDogsAsync(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedDogs = Assert.IsAssignableFrom<PaginatedList<Dog>>(okResult.Value);
            Assert.Single(returnedDogs.Items);
            Assert.Equal("PesPatron", returnedDogs.Items.First().Name);
        }

        [Fact]
        public async Task GetAllDogsAsync_InvalidQuery_ReturnsBadRequest()
        {
            // Arrange
            var query = new DogQueryObject { PageNumber = 1, PageSize = 2, Attribute = "sakdoaskdoqkwdokqw", Order = "asc" };

            // Act
            var result = await _dogController.GetAllDogsAsync(query);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid sorting attribute or order.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAllDogsAsync_NoDogsFound_ReturnsNotFound()
        {
            // Arrange
            var query = new DogQueryObject { PageNumber = 1, PageSize = 2, Attribute = "name", Order = "asc" };
            var emptyDogs = new PaginatedList<Dog>(new List<Dog>(), 0, 1, 2);

            _mockDogRepository.Setup(repo => repo.GetAllAsync(query)).ReturnsAsync(emptyDogs);

            // Act
            var result = await _dogController.GetAllDogsAsync(query);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No dogs found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAllDogsAsync_InternalServerError_ReturnsStatusCode500()
        {
            // Arrange
            var query = new DogQueryObject { PageNumber = 1, PageSize = 2, Attribute = "name", Order = "asc" };
            _mockDogRepository.Setup(repo => repo.GetAllAsync(query)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _dogController.GetAllDogsAsync(query);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Internal server error: Some error", statusCodeResult.Value.ToString());
        }

        [Fact]
        public async Task CreateDogAsync_ValidDog_ReturnsOkResult()
        {
            // Arrange
            var dogCreateDTO = new DogCreateDTO
            {
                Name = "PesPatron",
                Color = "Brown",
                TailLength = 10,
                Weight = 30
            };

            _mockDogRepository.Setup(repo => repo.GetByNameAsync(dogCreateDTO.Name)).ReturnsAsync((Dog)null);

            // Act
            var result = await _dogController.CreateDogAsync(dogCreateDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdDog = Assert.IsAssignableFrom<Dog>(okResult.Value);
            Assert.Equal("PesPatron", createdDog.Name);
        }

        [Fact]
        public async Task CreateDogAsync_DogAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var dogCreateDTO = new DogCreateDTO
            {
                Name = "PesPatron",
                Color = "Brown",
                TailLength = 10,
                Weight = 30
            };

            var existingDog = new Dog { Name = "PesPatron" };
            _mockDogRepository.Setup(repo => repo.GetByNameAsync(dogCreateDTO.Name)).ReturnsAsync(existingDog);

            // Act
            var result = await _dogController.CreateDogAsync(dogCreateDTO);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal($"A dog with the name '{dogCreateDTO.Name}' already exists in DB.", conflictResult.Value);
        }

        [Fact]
        public async Task CreateDogAsync_NegativeTailLength_ReturnsBadRequest()
        {
            // Arrange
            var dogCreateDTO = new DogCreateDTO
            {
                Name = "PesPatron",
                Color = "Brown",
                TailLength = -1, // Invalid value
                Weight = 30
            };

            // Act
            var result = await _dogController.CreateDogAsync(dogCreateDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Tail length must be a non-negative number.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateDogAsync_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var dogCreateDTO = new DogCreateDTO
            {
                Name = "",
                Color = "Brown",
                TailLength = 10,
                Weight = 30
            };

            _dogController.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _dogController.CreateDogAsync(dogCreateDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid JSON is passed in a request body or missing required fields.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateDogAsync_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var dogCreateDTO = new DogCreateDTO
            {
                Name = "PesPatron",
                Color = "Brown",
                TailLength = 10,
                Weight = 30
            };

            _mockDogRepository.Setup(repo => repo.GetByNameAsync(dogCreateDTO.Name)).ReturnsAsync((Dog)null);
            _mockDogRepository.Setup(repo => repo.CreateAsync(It.IsAny<Dog>())).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _dogController.CreateDogAsync(dogCreateDTO);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Contains("Internal server error during dog creation: Some error", statusCodeResult.Value.ToString());
        }


    }
}
