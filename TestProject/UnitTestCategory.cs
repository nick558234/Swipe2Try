using FluentAssertions;
using Xunit;
using Moq;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;

namespace TestProject
{
    public class UnitTestCategory
    {
        private readonly Mock<ICategoryRepository> _mockRepo;
        private readonly ICategoryValidator _categoryValidator;
        private readonly CategoryManager _categoryManager;

        public UnitTestCategory()
        {
            _mockRepo = new Mock<ICategoryRepository>();
            _categoryValidator = new CategoryValidator();
            _categoryManager = new CategoryManager(_mockRepo.Object, _categoryValidator);
        }

        #region GetAllCategoriesAsync Tests

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnListOfCategories()
        {
            // Arrange
            var fakeCategories = new List<Category>
            {
                new() { Id = 1, Name = "Italian Cuisine", Photo = "https://example.com/italian.jpg" },
                new() { Id = 2, Name = "Mexican Food", Photo = "https://example.com/mexican.jpg" }
            };
            _mockRepo.Setup(repo => repo.GetAllCategoriesAsync()).ReturnsAsync(fakeCategories);

            // Act
            var result = await _categoryManager.GetAllCategoriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Italian Cuisine");
            result[1].Name.Should().Be("Mexican Food");
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ShouldReturnEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var emptyCategories = new List<Category>();
            _mockRepo.Setup(repo => repo.GetAllCategoriesAsync()).ReturnsAsync(emptyCategories);

            // Act
            var result = await _categoryManager.GetAllCategoriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetCategoryByIdAsync Tests

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = 1;
            var fakeCategory = new Category { Id = categoryId, Name = "Italian Cuisine", Photo = "https://example.com/italian.jpg" };
            _mockRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync(fakeCategory);

            // Act
            var result = await _categoryManager.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(categoryId);
            result.Name.Should().Be("Italian Cuisine");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 999;
            _mockRepo.Setup(repo => repo.GetCategoryByIdAsync(categoryId)).ReturnsAsync((Category?)null);

            // Act
            var result = await _categoryManager.GetCategoryByIdAsync(categoryId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldThrowException_WhenIdIsInvalid()
        {
            // Act & Assert
            await FluentActions.Invoking(() => _categoryManager.GetCategoryByIdAsync(0))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("Category ID must be a positive integer*");

            await FluentActions.Invoking(() => _categoryManager.GetCategoryByIdAsync(-1))
                .Should().ThrowAsync<ArgumentException>()
                .WithMessage("Category ID must be a positive integer*");
        }

        #endregion

        #region AddCategoryAsync Tests

        [Fact]
        public async Task AddCategoryAsync_ShouldSucceed_WithValidCategory()
        {
            // Arrange
            var category = new Category
            {
                Name = "Italian Cuisine",
                Photo = "https://example.com/italian.jpg"
            };

            // Act
            var result = await _categoryManager.AddCategoryAsync(category);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.AddCategoryAsync(category), Times.Once);
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldFail_WhenCategoryIsNull()
        {
            // Act
            var result = await _categoryManager.AddCategoryAsync(null!);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Category cannot be null");
            _mockRepo.Verify(r => r.AddCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldFail_WhenNameIsEmpty()
        {
            // Arrange
            var category = new Category
            {
                Name = "",
                Photo = "https://example.com/italian.jpg"
            };

            // Act
            var result = await _categoryManager.AddCategoryAsync(category);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Category name is required.");
            _mockRepo.Verify(r => r.AddCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldFail_WhenNameIsTooLong()
        {
            // Arrange
            var category = new Category
            {
                Name = new string('a', 101), // 101 characters, exceeds 100 limit
                Photo = "https://example.com/italian.jpg"
            };

            // Act
            var result = await _categoryManager.AddCategoryAsync(category);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Category name cannot exceed 100 characters");
            _mockRepo.Verify(r => r.AddCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldFail_WhenPhotoUrlIsTooLong()
        {
            // Arrange
            var category = new Category
            {
                Name = "Italian Cuisine",
                Photo = new string('a', 501) // 501 characters, exceeds 500 limit
            };

            // Act
            var result = await _categoryManager.AddCategoryAsync(category);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Photo URL cannot exceed 500 characters");
            _mockRepo.Verify(r => r.AddCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldFail_WhenPhotoUrlIsInvalid()
        {
            // Arrange
            var category = new Category
            {
                Name = "Italian Cuisine",
                Photo = "invalid-url"
            };

            // Act
            var result = await _categoryManager.AddCategoryAsync(category);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Photo URL is not a valid URL.");
            _mockRepo.Verify(r => r.AddCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task AddCategoryAsync_ShouldSucceed_WithNullPhoto()
        {
            // Arrange
            var category = new Category
            {
                Name = "Italian Cuisine",
                Photo = null
            };

            // Act
            var result = await _categoryManager.AddCategoryAsync(category);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.AddCategoryAsync(category), Times.Once);
        }

        #endregion

        #region UpdateCategoryAsync Tests

        [Fact]
        public async Task UpdateCategoryAsync_ShouldSucceed_WithValidCategory()
        {
            // Arrange
            var category = new Category
            {
                Id = 1,
                Name = "Updated Italian Cuisine",
                Photo = "https://example.com/updated-italian.jpg"
            };

            // Act
            var result = await _categoryManager.UpdateCategoryAsync(category);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.UpdateCategoryAsync(category), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldFail_WhenCategoryIsNull()
        {
            // Act
            var result = await _categoryManager.UpdateCategoryAsync(null!);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Category cannot be null");
            _mockRepo.Verify(r => r.UpdateCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ShouldFail_WhenIdIsInvalid()
        {
            // Arrange
            var category = new Category
            {
                Id = 0, // Invalid ID
                Name = "Italian Cuisine",
                Photo = "https://example.com/italian.jpg"
            };

            // Act
            var result = await _categoryManager.UpdateCategoryAsync(category);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("A valid Category ID is required for an update.");
            _mockRepo.Verify(r => r.UpdateCategoryAsync(It.IsAny<Category>()), Times.Never);
        }

        #endregion

        #region DeleteCategoryAsync Tests

        [Fact]
        public async Task DeleteCategoryAsync_ShouldSucceed_WithValidId()
        {
            // Arrange
            var categoryId = 1;

            // Act
            var result = await _categoryManager.DeleteCategoryAsync(categoryId);

            // Assert
            result.Success.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            _mockRepo.Verify(r => r.DeleteCategoryAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ShouldFail_WithInvalidId()
        {
            // Arrange
            var invalidId = -1;

            // Act
            var result = await _categoryManager.DeleteCategoryAsync(invalidId);

            // Assert
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Invalid category ID");
            _mockRepo.Verify(r => r.DeleteCategoryAsync(It.IsAny<int>()), Times.Never);
        }

        #endregion
    }
}