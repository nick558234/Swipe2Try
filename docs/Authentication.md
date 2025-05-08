# 🔑 Swipe2Try Authentication System

This document provides a detailed explanation of the authentication system implemented in the Swipe2Try application.

## 🏗️ Authentication Architecture

The authentication system follows the same layered architecture as the rest of the application:

1.  **Core Layer** - Contains interfaces, models, and business logic
2.  **Data Access Layer** - Implements repositories for data retrieval
3.  **Presentation Layer** - Handles user interface and session management

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
4.  **Session Management** (`login.cshtml.cs`):
    -   On successful authentication:
        -   `UserID`, `UserName`, and `RoleName` (fetched via `IRoleManager`) are stored in `HttpContext.Session`.
5.  **Redirection & Authorization**:
    -   User is redirected based on `RoleName`.
    -   Subsequent requests are handled by `AuthorizationMiddleware` (see [Authorization](Authorization.md) document).

## 💻 Code Examples

### Session Management (in `login.cshtml.cs` `OnPostAsync`)

```csharp
// ... (validation and authentication logic) ...

if (result.Success && result.User != null)
{
    // Fetch the role name
    var role = await _roleManager.GetRoleByIdAsync(result.User.RoleID);
    var roleNameToStore = role?.RoleName ?? "Unknown";

    // Store user info in session
    HttpContext.Session.SetString("UserID", result.User.UserID);
    HttpContext.Session.SetString("UserName", result.User.Name);
    HttpContext.Session.SetString("UserRole", roleNameToStore); // Store RoleName
    
    // Redirect based on role name
    if (roleNameToStore == "ADMIN")
        return RedirectToPage("/Admin/Index");
    else if (roleNameToStore == "OWNER")
        return RedirectToPage("/RestaurantOwner/Index");
    else
        return RedirectToPage("/swipe");
}
// ...
```

## ⚠️ Security Considerations

!!! warning "Current Implementation Limitations"
    The current implementation is for demonstration purposes and has security limitations:
    -   **Password Storage**: Passwords are currently stored in plain text. **This is not secure for production.**
    -   **Session Management**: Uses basic session-based authentication.
    -   **CSRF Protection**: Anti-forgery tokens are not explicitly detailed for all forms in this document.
    -   **Account Security**: No account lockout mechanisms after multiple failed login attempts.

!!! danger "Production Environment Requirements"
    For a production environment, the following improvements are **essential**:
    1.  **Password Hashing**: Implement a strong, salted hashing algorithm (e.g., Argon2, BCrypt, or PBKDF2).
    2.  **HTTPS**: Enforce HTTPS for all traffic, especially authentication.
    3.  **Anti-Forgery Tokens**: Ensure CSRF protection is implemented for all state-changing requests.
    4.  **Rate Limiting**: Implement rate limiting on login attempts to prevent brute-force attacks.
    5.  **Cookie Security**: Set `Secure` and `HttpOnly` flags on session cookies.
    6.  **Session Timeout**: Implement and configure appropriate session expiration policies.