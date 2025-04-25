# Swipe2Try User Authentication System

This document explains how the user authentication system works in the Swipe2Try application.

## Architecture

The authentication system follows a clean architecture approach with three main layers:

1. **Core Layer** (`Swipe2Try.Core`)
   - Contains domain models, interfaces, business logic, and validation
   - Independent of external frameworks and databases

2. **Data Access Layer** (`Swipe2Try.DAL`)
   - Implements data access using SQL Server
   - Depends on the Core layer interfaces

3. **Presentation Layer** (`Swipe2Try`)
   - Contains the Razor Pages UI
   - Depends on Core layer for business logic

## Components

### Core Layer

#### Models
- `User.cs` - User entity with properties matching the database schema:
  - UserID (PK, nvarchar(10))
  - Name (nvarchar(100))
  - Email (nvarchar(255))
  - Password (nvarchar(255))
  - RoleID (FK, nvarchar(10))

- `Role.cs` - Role entity with properties matching the database schema:
  - RoleID (PK, nvarchar(10))
  - RoleName (nvarchar(50))

#### Interfaces
- `IUserRepository.cs` - Interface for user data access operations:
  - GetUserByEmailAsync - Retrieves a user by email
  - CreateUserAsync - Creates a new user
  - EmailExistsAsync - Checks if an email already exists

- `IRoleRepository.cs` - Interface for role data access operations:
  - GetAllRolesAsync - Retrieves all available roles
  - GetRoleByIdAsync - Retrieves a role by ID
  - RoleExistsAsync - Checks if a role exists

- `IUserValidator.cs` - Interface for user input validation:
  - ValidateForRegistrationAsync - Validates user data for registration
  - ValidateForLogin - Validates login credentials

- `IUserManager.cs` - Interface for user business logic:
  - RegisterUserAsync - Registers a new user
  - AuthenticateUserAsync - Authenticates a user login

- `IRoleManager.cs` - Interface for role management:
  - GetAllRolesAsync - Gets all available roles
  - AssignRoleAsync - Assigns a role to a user
  - UpdateRoleAsync - Updates a user's role
  - DeleteRoleAsync - Deletes a role
  - GetUsersByRoleAsync - Gets all users with a specific role

#### Validation
- `UserValidator.cs` - Implements validation logic for user operations:
  - Validates required fields
  - Validates email format
  - Checks password length
  - Ensures email uniqueness
  - Validates role exists in the database

#### Managers
- `UserManager.cs` - Implements business logic for user operations:
  - Handles user registration process
  - Handles user authentication
  - Generates unique user IDs

- `RoleManager.cs` - Implements business logic for role operations:
  - Retrieves roles from the database
  - Handles role assignments for users
  - Manages role updates

### Data Access Layer

#### Repositories
- `UserRepository.cs` - Implements user data access operations:
  - Uses ADO.NET with SqlConnection and SqlCommand
  - Maps database results to domain models
  - Executes SQL queries against the USERS table

- `RoleRepository.cs` - Implements role data access operations:
  - Retrieves roles from the ROLES table
  - Checks for role existence
  - Maps database results to Role models

### Presentation Layer

#### Pages
- `login.cshtml` / `login.cshtml.cs` - Login page:
  - Displays login form
  - Handles form submission
  - Shows validation errors
  - Authenticates users using UserManager
  - Stores user info in session

- `SignUp.cshtml` / `SignUp.cshtml.cs` - Registration page:
  - Displays registration form with dropdown for roles
  - Fetches available roles from the database
  - Handles form submission
  - Shows validation errors
  - Registers users using UserManager

## Flow

### Registration Flow
1. User fills out the registration form with name, email, password, and selects a role from the dropdown
2. Form is submitted to the server
3. Input is validated by UserValidator, including validation that the selected role exists
4. If validation passes, UserManager generates a unique ID and creates the user
5. The user is redirected to the login page

### Login Flow
1. User provides email and password
2. Input is validated by UserValidator
3. UserManager retrieves the user from the database and verifies credentials
4. If authentication is successful, user info is stored in session
5. User is redirected to the home page

## Database Schema

### USERS Table
- UserID (PK, nvarchar(10), not null)
- Name (nvarchar(100), not null)
- Email (nvarchar(255), not null)
- Password (nvarchar(255), not null)
- RoleID (FK, nvarchar(10), null) - Foreign key to ROLES table

### ROLES Table
- RoleID (PK, nvarchar(10), not null)
- RoleName (nvarchar(50), not null)

## Configuration

The system is configured in `Program.cs` with dependency injection:
- Session support is enabled
- Repositories, validators, and managers are registered with the DI container

## Security Notes

This is a simple implementation for demonstration purposes:
- Passwords are stored in plain text (not recommended for production)
- Authentication is session-based with no token management
- No CSRF protection is implemented
- No account lockout mechanism is implemented

For a production system, implement:
- Password hashing using a secure algorithm
- HTTPS enforcement
- Anti-forgery tokens
- Rate limiting
- Account lockout after failed attempts 