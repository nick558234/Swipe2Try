using FluentAssertions;
using Xunit;
using Moq;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;


namespace TestProject
{
    public class UnitTestDish
    {
        private readonly Mock<IDishRepository> _mockRepo;
        private readonly IDishValidator _dishValidator;
        private readonly DishManager _dishManager;

        public UnitTestDish()
        {
            _mockRepo = new Mock<IDishRepository>();
            _dishValidator = new DishValidator();
            _dishManager = new DishManager(_mockRepo.Object, _dishValidator);
        }

        #region GetAllDishesAsync Tests

        [Fact]
        public async Task GetAllDishesAsync_ShouldReturnListOfDishes()
        {
            // Arrange  
            var fakeDishes = new List<Dish>
            {
                new() { Id = 1, Name = "TestDish1", Description = "Description1" },
                new() { Id = 2, Name = "TestDish2", Description = "Description2" }
            };
            _mockRepo.Setup(repo => repo.GetAllDishesAsync()).ReturnsAsync(fakeDishes);

            // Act  
            var result = await _dishManager.GetAllDishesAsync();

            // Assert  
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("TestDish1");
            result[1].Name.Should().Be("TestDish2");
        }

        [Fact]
        public async Task GetAllDishesAsync_ShouldReturnEmptyList_WhenNoDishesExist()
        {
            // Arrange
            var emptyDishes = new List<Dish>();
            _mockRepo.Setup(repo => repo.GetAllDishesAsync()).ReturnsAsync(emptyDishes);

            // Act
            var result = await _dishManager.GetAllDishesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetDishesByUserIdAsync Tests

        [Fact]
        public async Task GetDishesByUserIdAsync_ShouldReturnUserDishes()
        {
            // Arrange
            var userId = "user123";
            var userDishes = new List<Dish>
            {
                new() { Id = 1, Name = "User Dish 1", Description = "Description1", UserId = userId },
                new() { Id = 2, Name = "User Dish 2", Description = "Description2", UserId = userId }
            };
            _mockRepo.Setup(repo => repo.GetDishesByUserIdAsync(userId)).ReturnsAsync(userDishes);

            // Act
            var result = await _dishManager.GetDishesByUserIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.All(d => d.UserId == userId).Should().BeTrue();
        }

        [Fact]
        public async Task GetDishesByUserIdAsync_ShouldThrowException_WhenUserIdIsEmpty()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _dishManager.GetDishesByUserIdAsync(""))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("User ID cannot be null or empty*");
        }

        [Fact]
        public async Task GetDishesByUserIdAsync_ShouldThrowException_WhenUserIdIsNull()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _dishManager.GetDishesByUserIdAsync(null))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("User ID cannot be null or empty*");
        }

        #endregion

        #region GetDishByIdAsync Tests

        [Fact]
        public async Task GetDishByIdAsync_ShouldReturnDish_WhenDishExists()
        {
            // Arrange
            var dishId = 1;
            var expectedDish = new Dish { Id = dishId, Name = "Test Dish", Description = "Test Description" };
            _mockRepo.Setup(repo => repo.GetDishByIdAsync(dishId)).ReturnsAsync(expectedDish);

            // Act
            var result = await _dishManager.GetDishByIdAsync(dishId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(dishId);
            result.Name.Should().Be("Test Dish");
        }

        [Fact]
        public async Task GetDishByIdAsync_ShouldReturnNull_WhenDishDoesNotExist()
        {
            // Arrange
            var dishId = 999;
            _mockRepo.Setup(repo => repo.GetDishByIdAsync(dishId)).ReturnsAsync((Dish?)null);

            // Act
            var result = await _dishManager.GetDishByIdAsync(dishId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetDishByIdAsync_ShouldThrowException_WhenIdIsZero()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _dishManager.GetDishByIdAsync(0))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("Dish ID must be a positive integer*");
        }

        [Fact]
        public async Task GetDishByIdAsync_ShouldThrowException_WhenIdIsNegative()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _dishManager.GetDishByIdAsync(-1))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("Dish ID must be a positive integer*");
        }

        #endregion

        #region AddDishAsync Tests

        [Fact]
        public async Task AddDishAsync_ShouldSucceed_WhenDishIsValid()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5,
                Photo = "pasta.jpg",
                Restaurant = "Italian Bistro"
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.AddDishAsync(dish), Times.Once);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenHealthFactorIsGreaterThan10()
        {
            // Arrange  
            var dish = new Dish
            {
                Id = 1,
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "1",
                HealthFactor = 1000, // Invalid 
                Photo = "pasta.jpg",
                Restaurant = "Italian Bistro",
            };

            // Act  
            var result = await _dishManager.AddDishAsync(dish);

            // Assert  
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Health factor must be between 1 and 10");
            // test data never reaches the database
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenHealthFactorIsLessThan1()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 0, // Invalid
                Photo = "pasta.jpg",
                Restaurant = "Italian Bistro"
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Health factor must be between 1 and 10");
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenNameIsEmpty()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "", // Invalid
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish name is required");
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenNameIsTooLong()
        {
            // Arrange
            var dish = new Dish
            {
                Name = new string('A', 101), // Invalid - too long
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish name cannot exceed 100 characters");
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenDescriptionIsEmpty()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = "", // Invalid
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish description is required");
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenDescriptionIsTooLong()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = new string('A', 501), // Invalid - too long
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish description cannot exceed 500 characters");
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenDishIsNull()
        {
            // Act
            var result = await _dishManager.AddDishAsync(null!);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish cannot be null");
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task AddDishAsync_ShouldSucceed_WhenHealthFactorIsNull()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = null, // Optional field
                Photo = "pasta.jpg",
                Restaurant = "Italian Bistro"
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.AddDishAsync(dish), Times.Once);
        }

        [Fact]
        public async Task AddDishAsync_ShouldFail_WhenPhotoUrlIsTooLong()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5,
                Photo = new string('A', 501), // Invalid - too long
                Restaurant = "Italian Bistro"
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Photo URL cannot exceed 500 characters");
            _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        #endregion

        #region UpdateDishAsync Tests

        [Fact]
        public async Task UpdateDishAsync_ShouldSucceed_WhenDishIsValid()
        {
            // Arrange
            var dish = new Dish
            {
                Id = 1,
                Name = "Updated Pasta",
                Description = "Updated delicious pasta",
                UserId = "user123",
                HealthFactor = 6,
                Photo = "updated_pasta.jpg",
                Restaurant = "Italian Bistro"
            };

            // Act
            var result = await _dishManager.UpdateDishAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.UpdateDishAsync(dish), Times.Once);
        }

        [Fact]
        public async Task UpdateDishAsync_ShouldFail_WhenIdIsZero()
        {
            // Arrange
            var dish = new Dish
            {
                Id = 0, // Invalid for update
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.UpdateDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish ID must be a positive integer for update");
            _mockRepo.Verify(r => r.UpdateDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDishAsync_ShouldFail_WhenIdIsNegative()
        {
            // Arrange
            var dish = new Dish
            {
                Id = -1, // Invalid for update
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.UpdateDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish ID must be a positive integer for update");
            _mockRepo.Verify(r => r.UpdateDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDishAsync_ShouldFail_WhenDishIsNull()
        {
            // Act
            var result = await _dishManager.UpdateDishAsync(null!);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Dish cannot be null");
            _mockRepo.Verify(r => r.UpdateDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDishAsync_ShouldFail_WhenValidationFailsForMultipleReasons()
        {
            // Arrange
            var dish = new Dish
            {
                Id = -1, // Invalid ID
                Name = "", // Invalid name
                Description = "", // Invalid description
                HealthFactor = 15 // Invalid health factor
            };

            // Act
            var result = await _dishManager.UpdateDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThan(1);
            result.Errors.Should().Contain("Dish ID must be a positive integer for update");
            result.Errors.Should().Contain("Dish name is required");
            result.Errors.Should().Contain("Dish description is required");
            result.Errors.Should().Contain("Health factor must be between 1 and 10");
            _mockRepo.Verify(r => r.UpdateDishAsync(It.IsAny<Dish>()), Times.Never);
        }

        #endregion

        #region DeleteDishAsync Tests

        [Fact]
        public async Task DeleteDishAsync_ShouldSucceed_WhenIdIsValid()
        {
            // Arrange
            var dishId = 1;

            // Act
            var result = await _dishManager.DeleteDishAsync(dishId);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.DeleteDishAsync(dishId), Times.Once);
        }

        [Fact]
        public async Task DeleteDishAsync_ShouldFail_WhenIdIsZero()
        {
            // Act
            var result = await _dishManager.DeleteDishAsync(0);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Invalid dish ID");
            _mockRepo.Verify(r => r.DeleteDishAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDishAsync_ShouldFail_WhenIdIsNegative()
        {
            // Act
            var result = await _dishManager.DeleteDishAsync(-1);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Invalid dish ID");
            _mockRepo.Verify(r => r.DeleteDishAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion

        #region WithMessage Methods Tests

        [Fact]
        public async Task AddDishWithMessageAsync_ShouldReturnSuccessMessage_WhenDishIsValid()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishWithMessageAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Dish created successfully!");
        }

        [Fact]
        public async Task AddDishWithMessageAsync_ShouldReturnErrorMessage_WhenDishIsInvalid()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "", // Invalid
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishWithMessageAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Dish name is required");
        }

        [Fact]
        public async Task UpdateDishWithMessageAsync_ShouldReturnSuccessMessage_WhenDishIsValid()
        {
            // Arrange
            var dish = new Dish
            {
                Id = 1,
                Name = "Updated Pasta",
                Description = "Updated delicious pasta",
                UserId = "user123",
                HealthFactor = 6
            };

            // Act
            var result = await _dishManager.UpdateDishWithMessageAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Dish updated successfully!");
        }

        [Fact]
        public async Task UpdateDishWithMessageAsync_ShouldReturnErrorMessage_WhenDishIsInvalid()
        {
            // Arrange
            var dish = new Dish
            {
                Id = 0, // Invalid
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.UpdateDishWithMessageAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Dish ID must be a positive integer for update");
        }

        [Fact]
        public async Task DeleteDishWithMessageAsync_ShouldReturnSuccessMessage_WhenIdIsValid()
        {
            // Arrange
            var dishId = 1;

            // Act
            var result = await _dishManager.DeleteDishWithMessageAsync(dishId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Dish deleted successfully!");
        }

        [Fact]
        public async Task DeleteDishWithMessageAsync_ShouldReturnErrorMessage_WhenIdIsInvalid()
        {
            // Act
            var result = await _dishManager.DeleteDishWithMessageAsync(0);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Invalid dish ID");
        }

        #endregion

        #region Repository Exception Handling Tests

        [Fact]
        public async Task AddDishAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            _mockRepo.Setup(r => r.AddDishAsync(It.IsAny<Dish>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Failed to add dish: Database connection failed");
        }

        [Fact]
        public async Task UpdateDishAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            var dish = new Dish
            {
                Id = 1,
                Name = "Pasta",
                Description = "Delicious pasta",
                UserId = "user123",
                HealthFactor = 5
            };

            _mockRepo.Setup(r => r.UpdateDishAsync(It.IsAny<Dish>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _dishManager.UpdateDishAsync(dish);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Failed to update dish: Database connection failed");
        }

        [Fact]
        public async Task DeleteDishAsync_ShouldHandleRepositoryException()
        {
            // Arrange
            _mockRepo.Setup(r => r.DeleteDishAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _dishManager.DeleteDishAsync(1);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Failed to delete dish: Database connection failed");
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public async Task AddDishAsync_ShouldSucceed_WithValidHealthFactorBoundaryValues()
        {
            // Test minimum valid health factor
            var dishMin = new Dish
            {
                Name = "Healthy Salad",
                Description = "Very healthy salad",
                UserId = "user123",
                HealthFactor = 1 // Minimum valid value
            };

            var resultMin = await _dishManager.AddDishAsync(dishMin);
            resultMin.Success.Should().BeTrue();

            // Test maximum valid health factor
            var dishMax = new Dish
            {
                Name = "Super Healthy Dish",
                Description = "Extremely healthy dish",
                UserId = "user123",
                HealthFactor = 10 // Maximum valid value
            };

            var resultMax = await _dishManager.AddDishAsync(dishMax);
            resultMax.Success.Should().BeTrue();
        }

        [Fact]
        public async Task AddDishAsync_ShouldSucceed_WithMaximumValidNameLength()
        {
            // Arrange
            var dish = new Dish
            {
                Name = new string('A', 100), // Maximum valid length
                Description = "Test description",
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task AddDishAsync_ShouldSucceed_WithMaximumValidDescriptionLength()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Test Dish",
                Description = new string('A', 500), // Maximum valid length
                UserId = "user123",
                HealthFactor = 5
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task AddDishAsync_ShouldSucceed_WithMaximumValidPhotoUrlLength()
        {
            // Arrange
            var dish = new Dish
            {
                Name = "Test Dish",
                Description = "Test description",
                UserId = "user123",
                HealthFactor = 5,
                Photo = new string('A', 500) // Maximum valid length
            };

            // Act
            var result = await _dishManager.AddDishAsync(dish);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        #endregion
    }
}