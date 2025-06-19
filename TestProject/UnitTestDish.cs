using FluentAssertions;
using Xunit;
using Moq;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;

namespace TestProject;

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

    #region GetDishByIdAsync Tests

    [Fact]
    public async Task GetDishByIdAsync_ShouldReturnDish_WhenDishExists()
    {
        // Arrange
        var dishId = 1;
        var fakeDish = new Dish { Id = dishId, Name = "TestDish", Description = "TestDescription" };
        _mockRepo.Setup(repo => repo.GetDishByIdAsync(dishId)).ReturnsAsync(fakeDish);

        // Act
        var result = await _dishManager.GetDishByIdAsync(dishId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(dishId);
        result.Name.Should().Be("TestDish");
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
    public async Task GetDishesByUserIdAsync_ShouldReturnEmptyList_WhenUserHasNoDishes()
    {
        // Arrange
        var userId = "user456";
        var emptyDishes = new List<Dish>();
        _mockRepo.Setup(repo => repo.GetDishesByUserIdAsync(userId)).ReturnsAsync(emptyDishes);

        // Act
        var result = await _dishManager.GetDishesByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDishesByUserIdAsync_ShouldThrowException_WhenUserIdIsEmpty()
    {
        // Act & Assert
        await FluentActions.Invoking(() => _dishManager.GetDishesByUserIdAsync(""))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("User ID cannot be null or empty*");
    }

    #endregion

    #region AddDishAsync Tests

    [Fact]
    public async Task AddDishAsync_ShouldSucceed_WithValidDish()
    {
        // Arrange
        var dish = new Dish
        {
            Name = "Valid Dish",
            Description = "Valid Description",
            UserId = "user123",
            HealthFactor = 5
        };

        // Act
        var result = await _dishManager.AddDishAsync(dish);

        // Assert
        result.Success.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockRepo.Verify(r => r.AddDishAsync(dish), Times.Once);
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
    public async Task AddDishAsync_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var dish = new Dish
        {
            Name = "",
            Description = "Valid Description",
            UserId = "user123",
            HealthFactor = 5
        }; // Act
        var result = await _dishManager.AddDishAsync(dish);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Dish name is required");
        _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
    }

    [Fact]
    public async Task AddDishAsync_ShouldFail_WhenDescriptionIsEmpty()
    {
        // Arrange
        var dish = new Dish
        {
            Name = "Test Dish",
            Description = "",
            UserId = "user123",
            HealthFactor = 5
        }; // Act
        var result = await _dishManager.AddDishAsync(dish);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Dish description is required");
        _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
    }

    [Fact]
    public async Task AddDishAsync_ShouldFail_WhenNameIsTooLong()
    {
        // Arrange
        var dish = new Dish
        {
            Name = new string('a', 101), // 101 characters, exceeds 100 limit
            Description = "Valid Description",
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
    public async Task AddDishAsync_ShouldFail_WhenHealthFactorIsInvalid()
    {
        // Arrange
        var dish = new Dish
        {
            Name = "Test Dish",
            Description = "Valid Description",
            UserId = "user123",
            HealthFactor = 1000 // Invalid - should be 1-10
        };

        // Act
        var result = await _dishManager.AddDishAsync(dish);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Health factor must be between 1 and 10");
        _mockRepo.Verify(r => r.AddDishAsync(It.IsAny<Dish>()), Times.Never);
    }

    [Fact]
    public async Task AddDishAsync_ShouldSucceed_WithBoundaryHealthFactors()
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

    #endregion

    #region UpdateDishAsync Tests

    [Fact]
    public async Task UpdateDishAsync_ShouldSucceed_WithValidDish()
    {
        // Arrange
        var dish = new Dish
        {
            Id = 1,
            Name = "Updated Dish",
            Description = "Updated Description",
            UserId = "user123",
            HealthFactor = 8
        };

        // Act
        var result = await _dishManager.UpdateDishAsync(dish);

        // Assert
        result.Success.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockRepo.Verify(r => r.UpdateDishAsync(dish), Times.Once);
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
    public async Task UpdateDishAsync_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var dish = new Dish
        {
            Id = 1,
            Name = "",
            Description = "Valid Description",
            UserId = "user123",
            HealthFactor = 5
        };

        // Act
        var result = await _dishManager.UpdateDishAsync(dish); // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Dish name is required");
        _mockRepo.Verify(r => r.UpdateDishAsync(It.IsAny<Dish>()), Times.Never);
    }

    #endregion

    #region DeleteDishAsync Tests

    [Fact]
    public async Task DeleteDishAsync_ShouldSucceed_WithValidId()
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
    public async Task DeleteDishAsync_ShouldFail_WithInvalidId()
    {
        // Arrange
        var invalidId = -1;

        // Act
        var result = await _dishManager.DeleteDishAsync(invalidId);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Invalid dish ID");
        _mockRepo.Verify(r => r.DeleteDishAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion
}