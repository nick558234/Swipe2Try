# Swipe2Try User Authentication & Data Layer System

This document explains how the user authentication system and data layers work in the Swipe2Try application.

## Architecture Overview

The application follows a clean architecture approach with three main layers:

1. **Core Layer** (`Swipe2Try.Core`)
   - Contains domain models, interfaces, business logic, and validation
   - Independent of external frameworks and databases

2. **Data Access Layer** (`Swipe2Try.DAL`)
   - Implements data access using repositories
   - Depends on the Core layer interfaces
   - Isolates database implementation details

3. **Presentation Layer** (`Swipe2Try`)
   - Contains the Razor Pages UI
   - Depends on Core layer for business logic
   - Uses the DAL through dependency injection

## Core Layer Components

### Domain Models
- `User.cs` - User entity with properties:
  - UserID (PK, nvarchar(10))
  - Name (nvarchar(100))
  - Email (nvarchar(255))
  - Password (nvarchar(255))
  - RoleID (FK, nvarchar(10))

- `Role.cs` - Role entity with properties:
  - RoleID (PK, nvarchar(10))
  - RoleName (nvarchar(50))

- `Dish.cs` - Dish entity with properties:
  - DishID (PK, nvarchar(10))
  - Name (nvarchar(100))
  - Description (nvarchar(255))
  - Price (decimal)
  - ImageUrl (nvarchar(255))
  - RestaurantID (FK, nvarchar(10))
  - Category (nvarchar(50))
  - HealthFactor (int)
  - IsActive (bit)

### Interfaces
- `IUserRepository.cs` - Interface for user data access operations
- `IRoleRepository.cs` - Interface for role data access operations
- `IDishRepository.cs` - Interface for dish data access operations
- `IUserValidator.cs` - Interface for user input validation
- `IUserManager.cs` - Interface for user business logic
- `IRoleManager.cs` - Interface for role management

### Validation & Business Logic
- `UserValidator.cs` - Implements validation logic for user operations
- `UserManager.cs` - Implements business logic for user operations
- `RoleManager.cs` - Implements business logic for role operations

## Data Access Layer Components

The DAL implements the Core layer interfaces and provides the actual data access logic.

### Key Repositories
- `UserRepository.cs` - Implements `IUserRepository`
  - Handles CRUD operations for users
  - Maps database results to User domain models
  - Manages user authentication data

- `RoleRepository.cs` - Implements `IRoleRepository`
  - Manages role data and user-role relationships
  - Supports role-based access control

- `DishRepository.cs` - Implements `IDishRepository`
  - Handles dish operations (creation, retrieval, updates)
  - Supports filtering, sorting, and search operations
  - Maps between database entities and domain models

### Data Access Implementation
- Repositories use SQL commands to interact with the database
- Each repository handles transactions for its specific domain entity
- Repositories are designed to be testable with dependency injection

## Authentication & Data Flow

### Registration Process
1. User submits registration form with personal details and role selection
2. `UserValidator` validates the input data
3. `UserManager` generates a unique ID and prepares the user object
4. `UserRepository` saves the new user to the database
5. User is redirected to login

### Login Process
1. User enters credentials (email/password)
2. `UserValidator` validates the format
3. `UserManager` requests authentication from `UserRepository`
4. `UserRepository` verifies credentials against the database
5. Upon success, user session is created with role information
6. User is redirected based on their role

### Dish Management Flow
1. Restaurant owner creates a new dish through the UI
2. Input is validated
3. `DishRepository` saves the dish to the database
4. When users browse dishes, `DishRepository` retrieves filtered data
5. Presentation layer displays dishes with "swipe" interaction

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

### DISHES Table
- DishID (PK, nvarchar(10), not null)
- Name (nvarchar(100), not null)
- Description (nvarchar(255), null)
- Price (decimal(10,2), not null)
- ImageUrl (nvarchar(255), null)
- RestaurantID (FK, nvarchar(10), not null)
- Category (nvarchar(50), null)
- HealthFactor (int, null)
- IsActive (bit, not null, default 1)
- CreatedDate (datetime, not null)
- ModifiedDate (datetime, null)

### RESTAURANTS Table
- RestaurantID (PK, nvarchar(10), not null)
- Name (nvarchar(100), not null)
- OwnerID (FK, nvarchar(10), not null) - Foreign key to USERS table
- Address (nvarchar(255), null)
- Phone (nvarchar(20), null)
- IsActive (bit, not null, default 1)

### USER_DISH_INTERACTIONS Table
- InteractionID (PK, nvarchar(10), not null)
- UserID (FK, nvarchar(10), not null)
- DishID (FK, nvarchar(10), not null)
- InteractionType (nvarchar(20), not null) - 'LIKE', 'DISLIKE', 'SAVED'
- InteractionDate (datetime, not null)

## Data Layer Best Practices

1. **Repository Pattern**: Each entity type has its own repository with specific methods
2. **Dependency Injection**: Repositories are injected into page models
3. **Asynchronous Operations**: Database operations use async/await pattern
4. **Error Handling**: Repository methods use try/catch and proper error propagation
5. **Transaction Management**: Multiple related operations use transactions
6. **Parameterized Queries**: All SQL uses parameters to prevent SQL injection

## Security Considerations

Current implementation is for demonstration purposes with some limitations:
- Passwords are stored in plain text (not recommended for production)
- Authentication is session-based without token management
- No CSRF protection is implemented
- No account lockout mechanism is implemented

For a production system, implement:
- Password hashing (Argon2, BCrypt, or PBKDF2)
- HTTPS enforcement
- Anti-forgery tokens
- Rate limiting
- Account lockout after failed attempts
- Input sanitization
- Proper error handling that doesn't expose sensitive information

## Future Data Layer Enhancements

1. **Entity Framework Core**: Consider migrating to EF Core for more robust ORM capabilities
2. **Caching Layer**: Add caching for frequently accessed data
3. **Audit Trails**: Implement logging of data changes
4. **Soft Delete**: Add soft delete functionality instead of permanent deletion
5. **Unit of Work Pattern**: Add transaction coordination across repositories
6. **Pagination**: Implement efficient pagination for large datasets