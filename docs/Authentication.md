# 🔑 Swipe2Try Authentication System

This document provides a detailed explanation of the authentication system implemented in the Swipe2Try application.

## 🏗️ Authentication Architecture

The authentication system follows the same layered architecture as the rest of the application:

1.  **Core Layer** - Contains interfaces, models, and business logic
2.  **Data Access Layer** - Implements repositories for data retrieval
3.  **Presentation Layer** - Handles user interface and cookie-based authentication

### Core Layer Components

#### Models

-   **`User.cs`** - Represents a user in the system.
    ```csharp
    // Swipe2Try.Core/Models/User.cs
    public class User
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } // Used for login
        public string Password { get; set; }
        public string RoleID { get; set; } // Reference to the user's role
    }
    ```

-   **`Role.cs`** - Represents a role in the system.
    ```csharp
    // Swipe2Try.Core/Models/Role.cs
    public class Role
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; } // e.g., "Admin", "RestaurantOwner", "User"
    }
    ```

#### Interfaces

-   **`IUserRepository.cs`** - Interface for user data access.
    ```csharp
    // Swipe2Try.Core/Interfaces/IUserRepository.cs
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email); // Retrieves a user by email
        Task<bool> CreateUserAsync(User user);        // Creates a new user
        Task<bool> EmailExistsAsync(string email);    // Checks if an email exists
    }
    ```

-   **`IRoleRepository.cs`** - Interface for role data access.
    ```csharp
    // Swipe2Try.Core/Interfaces/IRoleRepository.cs
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(string roleId);
    }
    ```

-   **`IUserValidator.cs`** - Validates user input.
    ```csharp
    // Swipe2Try.Core/Interfaces/IUserValidator.cs
    public interface IUserValidator
    {
        Task<(bool IsValid, string ErrorMessage)> ValidateForRegistrationAsync(User user);
        (bool IsValid, string ErrorMessage) ValidateForLogin(string email, string password);
    }
    ```

-   **`IUserManager.cs`** - Manages user operations.
    ```csharp
    // Swipe2Try.Core/Interfaces/IUserManager.cs
    public interface IUserManager
    {
        Task<(bool Success, string ErrorMessage)> RegisterUserAsync(User user);
        Task<(bool Success, User User, string ErrorMessage)> AuthenticateUserAsync(string email, string password);
    }
    ```

### Implementation Classes

#### Validation

-   **`UserValidator.cs`** - Implements `IUserValidator`.
    ```csharp
    // Swipe2Try.Core/Validation/UserValidator.cs
    public async Task<(bool IsValid, string ErrorMessage)> ValidateForRegistrationAsync(User user)
    {
        // Validates required fields, email format, password length
        // Ensures email uniqueness and role exists
        // ...implementation details...
    }
    
    public (bool IsValid, string ErrorMessage) ValidateForLogin(string email, string password)
    {
        // Validates email and password format
        // ...implementation details...
    }
    ```

#### Managers

-   **`UserManager.cs`** - Implements `IUserManager`.
    ```csharp
    // Swipe2Try.Core/Managers/UserManager.cs
    public async Task<(bool Success, string ErrorMessage)> RegisterUserAsync(User user)
    {
        // Validates user data
        // Generates unique ID
        // Creates user in repository
        // ...implementation details...
    }
    
    public async Task<(bool Success, User User, string ErrorMessage)> AuthenticateUserAsync(string email, string password)
    {
        // Validates credentials
        // Retrieves and verifies user
        // ...implementation details...
    }
    ```

#### Data Access

-   **`UserRepository.cs`** - Implements `IUserRepository`.
    ```csharp
    // Swipe2Try.DAL/Repositories/UserRepository.cs
    public async Task<User> GetUserByEmailAsync(string email)
    {
        // Executes SQL query to get user by email
        // Maps database results to User model
        // ...implementation details...
    }
    
    public async Task<bool> CreateUserAsync(User user)
    {
        // Executes SQL command to insert new user
        // ...implementation details...
    }
    ```

## 🔄 Authentication Flow

### Registration Flow

1.  **UI Layer** (`SignUp.cshtml.cs`):
    -   User fills out registration form (Name, Email, Password, Role).
    -   Form is submitted to the server.
2.  **Model Binding**: Data is bound to a `User` object.
3.  **Validation** (`UserValidator.ValidateForRegistrationAsync()`):
    -   Checks required fields, email format, password requirements.
    -   Ensures email uniqueness and selected role existence.
4.  **User Creation** (`UserManager.RegisterUserAsync()`):
    -   Generates unique `UserID`.
    -   Calls `UserRepository.CreateUserAsync()` to save the user.
5.  **Response**:
    -   ✅ Redirect to login page on success.
    -   ❌ Display validation errors if failed.

### Login Flow

1.  **UI Layer** (`login.cshtml.cs`):
    -   User enters email and password.
    -   Form is submitted to the server.
2.  **Validation** (`UserValidator.ValidateForLogin()`):
    -   Checks email format and password presence.
3.  **Authentication** (`UserManager.AuthenticateUserAsync()`):
    -   Calls `UserRepository.GetUserByEmailAsync()`.
    -   Verifies password.
    -   Returns user details including `RoleID`.
4.  **Identity Management** (`login.cshtml.cs`):
    -   On successful authentication:
        -   User identity information is stored in claims (Name, Email, Role).
        -   Claims are used to create a ClaimsIdentity.
        -   Authentication cookie is created with the ClaimsIdentity.
5.  **Redirection & Authorization**:
    -   User is redirected based on their role.
    -   Subsequent requests are authenticated via the cookie.

## 💻 Code Examples

### Cookie-Based Authentication (in `login.cshtml.cs` `OnPostAsync`)

```csharp
// ... (validation and authentication logic) ...

if (result.Success && result.User != null)
{
    // Get role from database
    var role = await _roleManager.GetRoleByIdAsync(result.User.RoleID);
    var roleNameToStore = role?.RoleName ?? "Unknown";

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, result.User.Name),
        new Claim(ClaimTypes.Email, result.User.Email),
        new Claim(ClaimTypes.Role, roleNameToStore)
    };
    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties
    {
        // Set cookie to expire after 5 minutes
        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
        // Make cookie persistent across browser sessions
        IsPersistent = true,
    };

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties);

    // Redirect based on role
    if (roleNameToStore.ToString() == "Admin")
        return RedirectToPage("/Admin/Index");
    else if (roleNameToStore.ToString() == "Restaurant Owner")
        return RedirectToPage("/RestaurantOwner/Index");
    else
        return RedirectToPage("/swipe");
}
// ...
```

### Logout Functionality (in `logout.cshtml.cs`)

```csharp
public async Task<IActionResult> OnGetAsync()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToPage("/Index");
}
```

### Cookie Authentication Configuration (in `Program.cs`)

```csharp
// Add built-in cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/Error";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Set cookie expiration to 5 minutes
        options.SlidingExpiration = true; // Reset expiration time with each request
    });
```

## Security Considerations

### Current Implementation

The application uses ASP.NET Core's built-in cookie authentication with the following configuration:

- **Cookie Lifetime**: 5 minutes with sliding expiration (resets with activity)
- **Persistent Cookies**: Cookies persist across browser sessions
- **Login Path**: Redirects to `/login` for unauthenticated requests
- **Logout Path**: `/logout` for signing out
- **Access Denied Path**: Redirects to `/Error` for unauthorized access