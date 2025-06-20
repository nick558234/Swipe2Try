using Moq;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestProject;

public class UnitTestCategoryManager
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Mock<IDishCategoryRepository> _mockDishCategoryRepo;
    private readonly Mock<IRestaurantCategoryRepository> _mockRestaurantCategoryRepo;
    private readonly ICategoryValidator _categoryValidator;
    private readonly CategoryManager _categoryManager;

    public UnitTestCategoryManager()
    {
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockDishCategoryRepo = new Mock<IDishCategoryRepository>();
        _mockRestaurantCategoryRepo = new Mock<IRestaurantCategoryRepository>();
        _categoryValidator = new CategoryValidator();
        _categoryManager = new CategoryManager(_mockCategoryRepo.Object, _mockDishCategoryRepo.Object, _mockRestaurantCategoryRepo.Object, _categoryValidator);
    }

    #region AssignCategoriesToDishAsync Tests

    [Fact]
    public async Task AssignCategoriesToDishAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var dishId = 1;
        var categoryIds = new List<int> { 1, 2, 3 };
        _mockDishCategoryRepo
            .Setup(x => x.UpdateDishCategoriesAsync(dishId, categoryIds))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryManager.AssignCategoriesToDishAsync(dishId, categoryIds);

        // Assert
        Xunit.Assert.True(result.Success);
        Xunit.Assert.Equal("Categories assigned successfully!", result.Message);
        _mockDishCategoryRepo.Verify(x => x.UpdateDishCategoriesAsync(dishId, categoryIds), Times.Once);
    }

    [Fact]
    public async Task AssignCategoriesToDishAsync_InvalidDishId_ReturnsFailure()
    {
        // Arrange
        var dishId = 0;
        var categoryIds = new List<int> { 1, 2, 3 };

        // Act
        var result = await _categoryManager.AssignCategoriesToDishAsync(dishId, categoryIds);

        // Assert
        Xunit.Assert.False(result.Success);
        Xunit.Assert.Equal("Invalid dish ID", result.Message);
        _mockDishCategoryRepo.Verify(x => x.UpdateDishCategoriesAsync(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task AssignCategoriesToDishAsync_EmptyCategoryList_ReturnsFailure()
    {
        // Arrange
        var dishId = 1;
        var categoryIds = new List<int>();

        // Act
        var result = await _categoryManager.AssignCategoriesToDishAsync(dishId, categoryIds);

        // Assert
        Xunit.Assert.False(result.Success);
        Xunit.Assert.Equal("No categories selected", result.Message);
        _mockDishCategoryRepo.Verify(x => x.UpdateDishCategoriesAsync(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
    }

    #endregion

    #region GetCategoriesForDishAsync Tests

    [Fact]
    public async Task GetCategoriesForDishAsync_ValidDishId_ReturnsCategories()
    {
        // Arrange
        var dishId = 1;
        var expectedCategories = new List<Category>
        {
            new Category { Id = 1, Name = "Italian" },
            new Category { Id = 2, Name = "Vegetarian" }
        };
        _mockDishCategoryRepo
            .Setup(x => x.GetCategoriesByDishIdAsync(dishId))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _categoryManager.GetCategoriesForDishAsync(dishId);

        // Assert
        Xunit.Assert.Equal(expectedCategories.Count, result.Count);
        Xunit.Assert.Equal(expectedCategories[0].Name, result[0].Name);
        _mockDishCategoryRepo.Verify(x => x.GetCategoriesByDishIdAsync(dishId), Times.Once);
    }

    [Fact]
    public async Task GetCategoriesForDishAsync_InvalidDishId_ThrowsException()
    {
        // Arrange
        var dishId = 0;

        // Act & Assert
        await Xunit.Assert.ThrowsAsync<System.ArgumentException>(() => _categoryManager.GetCategoriesForDishAsync(dishId));
        _mockDishCategoryRepo.Verify(x => x.GetCategoriesByDishIdAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region AssignCategoriesToRestaurantAsync Tests

    [Fact]
    public async Task AssignCategoriesToRestaurantAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var restaurantId = 1;
        var categoryIds = new List<int> { 1, 2, 3 };
        _mockRestaurantCategoryRepo
            .Setup(x => x.UpdateRestaurantCategoriesAsync(restaurantId, categoryIds))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryManager.AssignCategoriesToRestaurantAsync(restaurantId, categoryIds);

        // Assert
        Xunit.Assert.True(result.Success);
        Xunit.Assert.Equal("Categories assigned successfully!", result.Message);
        _mockRestaurantCategoryRepo.Verify(x => x.UpdateRestaurantCategoriesAsync(restaurantId, categoryIds), Times.Once);
    }

    [Fact]
    public async Task AssignCategoriesToRestaurantAsync_InvalidRestaurantId_ReturnsFailure()
    {
        // Arrange
        var restaurantId = 0;
        var categoryIds = new List<int> { 1, 2, 3 };

        // Act
        var result = await _categoryManager.AssignCategoriesToRestaurantAsync(restaurantId, categoryIds);

        // Assert
        Xunit.Assert.False(result.Success);
        Xunit.Assert.Equal("Invalid restaurant ID", result.Message);
        _mockRestaurantCategoryRepo.Verify(x => x.UpdateRestaurantCategoriesAsync(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
    }

    #endregion

    #region RemoveCategoryFromDishAsync Tests

    [Fact]
    public async Task RemoveCategoryFromDishAsync_ValidInput_ReturnsSuccess()
    {
        // Arrange
        var dishId = 1;
        var categoryId = 2;
        _mockDishCategoryRepo
            .Setup(x => x.UnlinkDishFromCategoryAsync(dishId, categoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryManager.RemoveCategoryFromDishAsync(dishId, categoryId);

        // Assert
        Xunit.Assert.True(result.Success);
        Xunit.Assert.Equal("Category removed successfully!", result.Message);
        _mockDishCategoryRepo.Verify(x => x.UnlinkDishFromCategoryAsync(dishId, categoryId), Times.Once);
    }

    #endregion
}
