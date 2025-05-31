using FluentAssertions;
using Xunit;
using Moq;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;


namespace TestProject
{    public class UnitTestDish
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
        }    [Fact]
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


    }
}