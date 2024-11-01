using Doghouse.Helpers;
using Doghouse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doghouse.Tests.HelperTests
{
    public class DogQueryObjectTests
    {
        private List<Dog> GetSampleDogs()
        {
            return new List<Dog>
        {
            new Dog { Id = 1, Name = "PesPatron", Color = "Brown", TailLength = 20, Weight = 30 },
            new Dog { Id = 2, Name = "Sharik", Color = "Black", TailLength = 15, Weight = 25 },
            new Dog { Id = 3, Name = "Bayraktar", Color = "White", TailLength = 10, Weight = 40 },
            new Dog { Id = 4, Name = "Stefania", Color = "Brown", TailLength = 12, Weight = 35 },
        };
        }


        [Fact]
        public void DefaultPageSize_IsCorrect()
        {
            // Arrange
            var queryObject = new DogQueryObject();

            // Act
            var pageSize = queryObject.PageSize;

            // Assert
            Assert.Equal(10, pageSize);
        }

        [Fact]
        public void PageSize_SettingAboveMax_SetsToMax()
        {
            // Arrange
            var queryObject = new DogQueryObject();

            // Act
            queryObject.PageSize = 100;

            // Assert
            Assert.Equal(50, queryObject.PageSize); 
        }

        [Fact]
        public void PageSize_SettingValidValue_SetsCorrectly()
        {
            // Arrange
            var queryObject = new DogQueryObject();

            // Act
            queryObject.PageSize = 20;

            // Assert
            Assert.Equal(20, queryObject.PageSize); 
        }

        [Fact]
        public void IsValid_WithValidAttribute_ReturnsTrue()
        {
            // Arrange
            var queryObject = new DogQueryObject { Attribute = "name" };

            // Act
            var isValid = queryObject.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_WithInvalidAttribute_ReturnsFalse()
        {
            // Arrange
            var queryObject = new DogQueryObject { Attribute = "invalid" };

            // Act
            var isValid = queryObject.IsValid();

            // Assert
            Assert.False(isValid); 
        }

        [Fact]
        public void IsValid_WithValidOrder_ReturnsTrue()
        {
            // Arrange
            var queryObject = new DogQueryObject { Order = "desc" };

            // Act
            var isValid = queryObject.IsValid();

            // Assert
            Assert.True(isValid); 
        }

        [Fact]
        public void IsValid_WithInvalidOrder_ReturnsFalse()
        {
            // Arrange
            var queryObject = new DogQueryObject { Order = "invalid" };

            // Act
            var isValid = queryObject.IsValid();

            // Assert
            Assert.False(isValid);
        }


        [Fact]
        public void ApplySorting_SortsByNameAscending()
        {
            // Arrange
            var queryObject = new DogQueryObject { Order = "name" };
            var dogs = GetSampleDogs().AsQueryable();

            // Act
            var sortedDogs = queryObject.ApplySorting(dogs).ToList();

            // Assert
            Assert.Equal("Bayraktar", sortedDogs[0].Name);
            Assert.Equal("PesPatron", sortedDogs[1].Name);
            Assert.Equal("Sharik", sortedDogs[2].Name);
            Assert.Equal("Stefania", sortedDogs[3].Name);
        }

        [Fact]
        public void ApplySorting_SortsByNameDescending()
        {
            // Arrange
            var queryObject = new DogQueryObject { Attribute = "name", Order = "desc" };
            var dogs = GetSampleDogs().AsQueryable();

            // Act
            var sortedDogs = queryObject.ApplySorting(dogs).ToList();

            // Assert
            Assert.Equal("Stefania", sortedDogs[0].Name);
            Assert.Equal("Sharik", sortedDogs[1].Name);
            Assert.Equal("PesPatron", sortedDogs[2].Name);
            Assert.Equal("Bayraktar", sortedDogs[3].Name);
        }

        [Fact]
        public void ApplySorting_SortsByColorAscending()
        {
            // Arrange
            var queryObject = new DogQueryObject { Attribute = "color" };
            var dogs = GetSampleDogs().AsQueryable();

            // Act
            var sortedDogs = queryObject.ApplySorting(dogs).ToList();

            // Assert
            Assert.Equal("Black", sortedDogs[0].Color);
            Assert.Equal("Brown", sortedDogs[1].Color);
            Assert.Equal("Brown", sortedDogs[2].Color);
            Assert.Equal("White", sortedDogs[3].Color);
        }

        [Fact]
        public void ApplySorting_SortsByTailLengthDescending()
        {
            // Arrange
            var queryObject = new DogQueryObject { Attribute = "taillength", Order = "desc" };
            var dogs = GetSampleDogs().AsQueryable();

            // Act
            var sortedDogs = queryObject.ApplySorting(dogs).ToList();

            // Assert
            Assert.Equal(20, sortedDogs[0].TailLength);
            Assert.Equal(15, sortedDogs[1].TailLength);
            Assert.Equal(12, sortedDogs[2].TailLength);
            Assert.Equal(10, sortedDogs[3].TailLength);
        }

        [Fact]
        public void ApplySorting_DefaultSortByName()
        {
            // Arrange
            var queryObject = new DogQueryObject();
            var dogs = GetSampleDogs().AsQueryable();

            // Act
            var sortedDogs = queryObject.ApplySorting(dogs).ToList();

            // Assert
            Assert.Equal("Bayraktar", sortedDogs[0].Name);
            Assert.Equal("PesPatron", sortedDogs[1].Name);
            Assert.Equal("Sharik", sortedDogs[2].Name);
            Assert.Equal("Stefania", sortedDogs[3].Name);
        }
    }
}
