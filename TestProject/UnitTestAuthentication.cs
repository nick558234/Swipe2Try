using FluentAssertions;
using Xunit;
using Moq;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;
using System.Security.Claims;

namespace TestProject;

/// <summary>
/// Comprehensive authentication unit tests using actual database role IDs.
/// Database roles: 1=Admin, 2=Restaurant Owner, 3=Customer
/// </summary>
public class UnitTestAuthentication
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IRoleRepository> _mockRoleRepo;
    private readonly RoleManager _roleManager;
    private readonly IUserValidator _userValidator;
    private readonly UserManager _userManager;

    public UnitTestAuthentication()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockRoleRepo = new Mock<IRoleRepository>();
        _roleManager = new RoleManager(_mockRoleRepo.Object, _mockUserRepo.Object);
        _userValidator = new UserValidator(_mockUserRepo.Object, _mockRoleRepo.Object);
        _userManager = new UserManager(_mockUserRepo.Object, _userValidator, _roleManager);
    }

    #region User Registration Tests (TC-001-01, TC-001-02)

    [Fact]
    public async Task RegisterUserAsync_ShouldSucceed_WithValidUserData()
    {
        // Arrange - TC-001-01: Verify successful user registration
        var user = new User
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "securePassword123",
            RoleID = "1" // Admin role from database
        };

        _mockUserRepo.Setup(repo => repo.EmailExistsAsync(user.Email)).ReturnsAsync(false);
        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(user.RoleID)).ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(true);

        // Act
        var result = await _userManager.RegisterUserAsync(user);

        // Assert
        result.Success.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WithInvalidEmail()
    {
        // Arrange - TC-001-02: Verify failed registration with invalid email
        var user = new User
        {
            Name = "John Doe",
            Email = "invalid-email-without-at-symbol",
            Password = "securePassword123",
            RoleID = "3" // Customer role from database
        };

        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(user.RoleID)).ReturnsAsync(true);

        // Act
        var result = await _userManager.RegisterUserAsync(user);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email format");
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenEmailIsMissingAtSymbol()
    {
        // Arrange - TC-001-02 variation: Email missing @ symbol
        var user = new User
        {
            Name = "John Doe",
            Email = "johndoeexample.com",
            Password = "securePassword123",
            RoleID = "3" // Customer role from database
        };

        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(user.RoleID)).ReturnsAsync(true);

        // Act
        var result = await _userManager.RegisterUserAsync(user);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email format");
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenUserIsNull()
    {
        // Act
        var result = await _userManager.RegisterUserAsync(null!);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var user = new User
        {
            Name = "",
            Email = "john.doe@example.com",
            Password = "securePassword123",
            RoleID = "3"
        };

        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(user.RoleID)).ReturnsAsync(true);

        // Act
        var result = await _userManager.RegisterUserAsync(user);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Name is required");
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenPasswordIsTooShort()
    {
        // Arrange
        var user = new User
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "123", // Too short
            RoleID = "3"
        };

        _mockUserRepo.Setup(repo => repo.EmailExistsAsync(user.Email)).ReturnsAsync(false);
        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(user.RoleID)).ReturnsAsync(true);

        // Act
        var result = await _userManager.RegisterUserAsync(user);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Password must be at least 6 characters");
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenEmailAlreadyExists()
    {
        // Arrange
        var user = new User
        {
            Name = "John Doe",
            Email = "existing@example.com",
            Password = "securePassword123",
            RoleID = "3"
        };

        _mockUserRepo.Setup(repo => repo.EmailExistsAsync(user.Email)).ReturnsAsync(true);
        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(user.RoleID)).ReturnsAsync(true);

        // Act
        var result = await _userManager.RegisterUserAsync(user);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Email is already registered");
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldFail_WhenRoleDoesNotExist()
    {
        // Arrange
        var user = new User
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "securePassword123",
            RoleID = "invalid-role"
        };

        _mockUserRepo.Setup(repo => repo.EmailExistsAsync(user.Email)).ReturnsAsync(false);
        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(user.RoleID)).ReturnsAsync(false);

        // Act
        var result = await _userManager.RegisterUserAsync(user);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Selected role is not valid");
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region User Login Tests (TC-001-03, TC-001-04)

    [Fact]
    public async Task LoginUserAsync_ShouldSucceed_WithValidCredentials()
    {
        // Arrange - TC-001-03: Verify successful user login
        var email = "john.doe@example.com";
        var password = "securePassword123";
        var hashedPassword =
            Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
        var user = new User
        {
            UserID = "user123",
            Name = "John Doe",
            Email = email,
            Password = hashedPassword,
            RoleID = "2"
        };
        var role = new Role { RoleID = "2", RoleName = "Restaurant Owner" };

        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);
        _mockRoleRepo.Setup(repo => repo.GetRoleByIdAsync(user.RoleID)).ReturnsAsync(role);

        // Act
        var result = await _userManager.LoginUserAsync(email, password);

        // Assert
        result.Success.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Principal.Should().NotBeNull();
        result.Principal!.Identity!.Name.Should().Be("John Doe");
        result.Principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == email);
        result.Principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Restaurant Owner");
    }

    [Fact]
    public async Task LoginUserAsync_ShouldFail_WithIncorrectPassword()
    {
        // Arrange - TC-001-04: Verify failed login with incorrect credentials
        var email = "john.doe@example.com";
        var correctPassword = "securePassword123";
        var incorrectPassword = "wrongPassword";
        var hashedPassword =
            Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(correctPassword)));

        var user = new User
        {
            UserID = "user123",
            Name = "John Doe",
            Email = email,
            Password = hashedPassword,
            RoleID = "3"
        };

        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _userManager.LoginUserAsync(email, incorrectPassword);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password");
        result.Principal.Should().BeNull();
    }

    [Fact]
    public async Task LoginUserAsync_ShouldFail_WithIncorrectEmail()
    {
        // Arrange - TC-001-04 variation: Verify failed login with incorrect email
        var incorrectEmail = "wrong@example.com";
        var password = "securePassword123";

        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(incorrectEmail)).ReturnsAsync((User?)null);

        // Act
        var result = await _userManager.LoginUserAsync(incorrectEmail, password);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email or password");
        result.Principal.Should().BeNull();
    }

    [Fact]
    public async Task LoginUserAsync_ShouldFail_WithEmptyEmail()
    {
        // Arrange
        var email = "";
        var password = "securePassword123";

        // Act
        var result = await _userManager.LoginUserAsync(email, password);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Email is required");
        result.Principal.Should().BeNull();
        _mockUserRepo.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginUserAsync_ShouldFail_WithEmptyPassword()
    {
        // Arrange
        var email = "john.doe@example.com";
        var password = "";

        // Act
        var result = await _userManager.LoginUserAsync(email, password);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Password is required");
        result.Principal.Should().BeNull();
        _mockUserRepo.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginUserAsync_ShouldFail_WithInvalidEmailFormat()
    {
        // Arrange
        var email = "invalid-email-format";
        var password = "securePassword123";

        // Act
        var result = await _userManager.LoginUserAsync(email, password);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email format");
        result.Principal.Should().BeNull();
        _mockUserRepo.Verify(r => r.GetUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    #endregion

    #region Authentication Helper Tests

    [Fact]
    public async Task AuthenticateUserAsync_ShouldSucceed_WithValidCredentials()
    {
        // Arrange
        var email = "test@example.com";
        var password = "testPassword123";
        var hashedPassword =
            Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));

        var user = new User
        {
            UserID = "user123",
            Name = "Test User",
            Email = email,
            Password = hashedPassword,
            RoleID = "3"
        };

        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _userManager.AuthenticateUserAsync(email, password);

        // Assert
        result.Success.Should().BeTrue();
        result.User.Should().NotBeNull();
        result.User!.Email.Should().Be(email);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task AuthenticateUserAsync_ShouldFail_WithNonExistentUser()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "password123";

        _mockUserRepo.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User?)null);

        // Act
        var result = await _userManager.AuthenticateUserAsync(email, password);

        // Assert
        result.Success.Should().BeFalse();
        result.User.Should().BeNull();
        result.Errors.Should().Contain("Invalid email or password");
    }

    #endregion

    #region Register and Login Combined Tests

    [Fact]
    public async Task RegisterAndLoginUserAsync_ShouldSucceed_WithValidData()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var password = "securePassword123";
        var roleId = "3";
        var role = new Role { RoleID = roleId, RoleName = "Customer" };

        _mockUserRepo.Setup(repo => repo.EmailExistsAsync(email)).ReturnsAsync(false);
        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(roleId)).ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(true);
        _mockRoleRepo.Setup(repo => repo.GetRoleByIdAsync(roleId)).ReturnsAsync(role);

        // Act
        var result = await _userManager.RegisterAndLoginUserAsync(name, email, password, roleId);

        // Assert
        result.Success.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Principal.Should().NotBeNull();
        result.Principal!.Identity!.Name.Should().Be(name);
        result.Principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == email);
        result.Principal.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Customer");
    }

    [Fact]
    public async Task RegisterAndLoginUserAsync_ShouldFail_WithInvalidEmail()
    {
        // Arrange
        var name = "John Doe";
        var email = "invalid-email";
        var password = "securePassword123";
        var roleId = "3";

        _mockRoleRepo.Setup(repo => repo.RoleExistsAsync(roleId)).ReturnsAsync(true);

        // Act
        var result = await _userManager.RegisterAndLoginUserAsync(name, email, password, roleId);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Invalid email format");
        result.Principal.Should().BeNull();
        _mockUserRepo.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    #endregion

    #region Role Redirect Tests

    [Fact]
    public void GetRedirectPageForRole_ShouldReturnCorrectPage_ForAdmin()
    {
        // Act
        var result = _userManager.GetRedirectPageForRole("Admin");

        // Assert
        result.Should().Be("/Admin/Index");
    }

    [Fact]
    public void GetRedirectPageForRole_ShouldReturnCorrectPage_ForRestaurantOwner()
    {
        // Act
        var result = _userManager.GetRedirectPageForRole("Restaurant Owner");

        // Assert
        result.Should().Be("/RestaurantOwner/Index");
    }

    [Fact]
    public void GetRedirectPageForRole_ShouldReturnDefaultPage_ForCustomer()
    {
        // Act
        var result = _userManager.GetRedirectPageForRole("Customer");

        // Assert
        result.Should().Be("/swipe");
    }

    [Fact]
    public void GetRedirectPageForRole_ShouldReturnDefaultPage_ForUnknownRole()
    {
        // Act
        var result = _userManager.GetRedirectPageForRole("UnknownRole");

        // Assert
        result.Should().Be("/swipe");
    }

    #endregion
}