using Doghouse.Data;
using Doghouse.Helpers;
using Doghouse.Models;
using Doghouse.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doghouse.Tests.RepositoryTests
{
    public class DogRepositoryTests
    {
        private readonly DogRepository _dogRepository;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DogRepositoryTests()
        {
            // Setup in-memory database options
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Create a new context using the in-memory database
            var context = new ApplicationDbContext(_options);
            _dogRepository = new DogRepository(context);
        }

        [Fact]
        public async Task CreateAsync_ValidDog_SavesAndReturnsDog()
        {
            // Arrange
            var dog = new Dog
            {
                Name = "PesPatron",
                Color = "Brown",
                TailLength = 10,
                Weight = 30
            };

            // Act
            var result = await _dogRepository.CreateAsync(dog);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PesPatron", result.Name);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_NullDog_ThrowsArgumentNullException()
        {
            // Arrange
            Dog nullDog = null;

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _dogRepository.CreateAsync(nullDog);
            });
        }

        [Fact]
        public async Task GetByNameAsync_ExistingDog_ReturnsDog()
        {
            // Arrange
            var dog = new Dog
            {
                Name = "PesPatron",
                Color = "Brown",
                TailLength = 10,
                Weight = 30
            };

            // Add dog to the in-memory database
            using (var context = new ApplicationDbContext(_options))
            {
                context.Dogs.Add(dog);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _dogRepository.GetByNameAsync("PesPatron");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("PesPatron", result.Name);
        }

        [Fact]
        public async Task GetByNameAsync_NonExistingDog_ReturnsNull()
        {
            // Act
            var result = await _dogRepository.GetByNameAsync("NonExistingDog");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ValidQuery_ReturnsPaginatedDogs()
        {
            // Arrange
            var dogs = new List<Dog>
        {
            new Dog { Name = "Dog1", Color = "Brown", TailLength = 10, Weight = 20 },
            new Dog { Name = "Dog2", Color = "Black", TailLength = 15, Weight = 25 },
            new Dog { Name = "Dog3", Color = "White", TailLength = 20, Weight = 30 }
        };

            using (var context = new ApplicationDbContext(_options))
            {
                context.Dogs.AddRange(dogs);
                await context.SaveChangesAsync();
            }

            var query = new DogQueryObject
            {
                PageNumber = 1,
                PageSize = 2,
                Attribute = "Name",
                Order = "asc"
            };

            // Act
            var result = await _dogRepository.GetAllAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count); 
            Assert.Equal(3, result.TotalCount); 
            Assert.Equal("Dog1", result.Items[0].Name); 
        }

        [Fact]
        public async Task GetAllAsync_NoDogs_ReturnsEmptyPaginatedList()
        {
            // Arrange
            var query = new DogQueryObject
            {
                PageNumber = 1,
                PageSize = 5
            };

            // Act
            var result = await _dogRepository.GetAllAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items); 
            Assert.Equal(0, result.TotalCount); 
        }

        [Fact]
        public async Task GetAllAsync_ValidQuery_ReturnsCorrectPagination()
        {
            // Arrange
            var dogs = new List<Dog>
        {
            new Dog { Name = "Dog1", Color = "Brown", TailLength = 10, Weight = 20 },
            new Dog { Name = "Dog2", Color = "Black", TailLength = 15, Weight = 25 },
            new Dog { Name = "Dog3", Color = "White", TailLength = 20, Weight = 30 },
            new Dog { Name = "Dog4", Color = "Golden", TailLength = 25, Weight = 35 }
        };

            using (var context = new ApplicationDbContext(_options))
            {
                context.Dogs.AddRange(dogs);
                await context.SaveChangesAsync();
            }

            var query = new DogQueryObject
            {
                PageNumber = 2, 
                PageSize = 2,  
                Attribute = "Name",
                Order = "asc"
            };

            // Act
            var result = await _dogRepository.GetAllAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(4, result.TotalCount);
            Assert.Equal("Dog3", result.Items[0].Name);
        }


    }
    }
