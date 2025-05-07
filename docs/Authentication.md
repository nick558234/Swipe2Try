# Swipe2Try Authentication System

This document provides a detailed explanation of the authentication system implemented in the Swipe2Try application.

## Authentication Architecture

The authentication system follows the same layered architecture as the rest of the application:

1. **Core Layer** - Contains interfaces, models, and business logic
2. **Data Access Layer** - Implements repositories for data retrieval
3. **Presentation Layer** - Handles user interface and session management

## Components

### Core Layer Components

#### Models
- `User.cs` - Represents a user in the system with the following properties:
  - UserID: Unique identifier
  - Name: Full name
  - Email: Email address (used for login)
  - Password: User password
  - RoleID: Reference to the user's role

- `Role.cs` - Represents a role in the system:
  - RoleID: Unique identifier
  - RoleName: Name of the role (e.g., "Admin", "RestaurantOwner", "User")

#### Interfaces
- `IUserRepository.cs` - Interface for user data access with methods:
  - `GetUserByEmailAsync(string email)`: Retrieves a user by email
  - `CreateUserAsync(User user)`: Creates a new user
  - `EmailExistsAsync(string email)`: Checks if an email exists

- `IRoleRepository.cs` - Interface for role data access with methods:
  - `GetAllRolesAsync()`: Retrieves all roles
  - `GetRoleByIdAsync(string roleId)`: Gets a role by ID

- `IUserValidator.cs` - Validates user input with methods:
  - `ValidateForRegistrationAsync(User user)`: Validates registration data
  - `ValidateForLogin(string email, string password)`: Validates login credentials

- `IUserManager.cs` - Manages user operations with methods:
  - `RegisterUserAsync(User user)`: Registers a new user
  - `AuthenticateUserAsync(string email, string password)`: Authenticates a user

### Implementation Classes

#### Validation
- `UserValidator.cs` - Implements `IUserValidator` with validation logic:
  ```csharp
  public async Task<(bool IsValid, string ErrorMessage)> ValidateForRegistrationAsync(User user)
  {
      // Validates required fields, email format, password length
      // Ensures email uniqueness and role exists
  }
  
  public (bool IsValid, string ErrorMessage) ValidateForLogin(string email, string password)
  {
      // Validates email and password format
  }
  ```

#### Managers
- `UserManager.cs` - Implements `IUserManager` with business logic:
  ```csharp
  public async Task<(bool Success, string ErrorMessage)> RegisterUserAsync(User user)
  {
      // Validates user data
      // Generates unique ID
      // Creates user in repository
  }
  
  public async Task<(bool Success, User User, string ErrorMessage)> AuthenticateUserAsync(string email, string password)
  {
      // Validates credentials
      // Retrieves and verifies user
  }
  ```

#### Data Access
- `UserRepository.cs` - Implements `IUserRepository`:
  ```csharp
  public async Task<User> GetUserByEmailAsync(string email)
  {
      // Executes SQL query to get user by email
      // Maps database results to User model
  }
  
  public async Task<bool> CreateUserAsync(User user)
  {
      // Executes SQL command to insert new user
  }
  ```

## Authentication Flow

### Registration Flow

1. **UI Layer**:
   - User fills out registration form (Name, Email, Password, Role)
   - Form is submitted to the server (`SignUp.cshtml.cs`)

2. **Model Binding**:
   - Data is bound to a User object

3. **Validation**:
   - `UserValidator.ValidateForRegistrationAsync()` checks:
     - Required fields are provided
     - Email format is valid
     - Password meets requirements
     - Email is not already in use
     - Selected role exists

4. **User Creation**:
   - `UserManager.RegisterUserAsync()` calls:
     - Generates unique UserID
     - `UserRepository.CreateUserAsync()` to save user

5. **Response**:
   - Redirect to login page on success
   - Display validation errors if failed

### Login Flow

1. **UI Layer**:
   - User enters email and password (`login.cshtml`)
   - Form is submitted to the server (`login.cshtml.cs`)

2. **Validation**:
   - `UserValidator.ValidateForLogin()` checks:
     - Email format
     - Password is provided

3. **Authentication**:
   - `UserManager.AuthenticateUserAsync()` calls:
     - `UserRepository.GetUserByEmailAsync()` to retrieve user
     - Verifies password matches
     - Returns user with role info

4. **Session Management**:
   - On successful authentication:
     - User info is stored in session (`HttpContext.Session`)
     - UserID, Name, and Role are saved

5. **Authorization**:
   - Custom middleware checks session for protected routes
   - Redirects based on role:
     - Admins to Admin dashboard
     - Restaurant owners to Restaurant management
     - Regular users to the main application

## Code Examples

### Session Management (Login Page Model)

```csharp
public async Task<IActionResult> OnPostAsync()
{
    // Validate input
    var validationResult = _userValidator.ValidateForLogin(Email, Password);
    if (!validationResult.IsValid)
    {
        ModelState.AddModelError(string.Empty, validationResult.ErrorMessage);
        return Page();
    }

    // Authenticate user
    var authResult = await _userManager.AuthenticateUserAsync(Email, Password);
    if (!authResult.Success)
    {
        ModelState.AddModelError(string.Empty, authResult.ErrorMessage);
        return Page();
    }

    // Set session variables
    HttpContext.Session.SetString("UserID", authResult.User.UserID);
    HttpContext.Session.SetString("UserName", authResult.User.Name);
    HttpContext.Session.SetString("UserRole", authResult.User.RoleID);

    // Redirect based on role
    if (authResult.User.RoleID == "ADMIN")
        return RedirectToPage("/Admin/Index");
    else if (authResult.User.RoleID == "OWNER")
        return RedirectToPage("/RestaurantOwner/Index");
    else
        return RedirectToPage("/Index");
}
```

### Authorization Middleware

The application uses custom middleware to protect routes based on roles:

```csharp
public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value.ToLower();
        
        // Check if path requires authentication
        if (path.Contains("/admin/") || path.Contains("/restaurantowner/"))
        {
            var userRole = context.Session.GetString("UserRole");
            
            // If not logged in, redirect to login
            if (string.IsNullOrEmpty(userRole))
            {
                context.Response.Redirect("/login");
                return;
            }
            
            // Check role-specific access
            if (path.Contains("/admin/") && userRole != "ADMIN")
            {
                context.Response.Redirect("/Error?code=403");
                return;
            }
            
            if (path.Contains("/restaurantowner/") && userRole != "OWNER")
            {
                context.Response.Redirect("/Error?code=403");
                return;
            }
        }
        
        await _next(context);
    }
}
```

## Security Considerations

The current implementation is for demonstration purposes with some limitations:

- **Password Storage**: Passwords are stored in plain text, which is not secure
- **Session Management**: Simple session-based authentication with no token management
- **CSRF Protection**: No anti-forgery tokens implemented
- **Account Security**: No account lockout after failed attempts

For a production environment, the following improvements should be implemented:

1. **Password Hashing**: Use a secure algorithm like Argon2, BCrypt, or PBKDF2
2. **HTTPS**: Enforce HTTPS for all authentication traffic
3. **Anti-Forgery Tokens**: Implement for all forms
4. **Rate Limiting**: Limit login attempts to prevent brute force attacks
5. **Cookie Security**: Set secure and HTTP-only flags on cookies
6. **Session Timeout**: Implement appropriate session expiration