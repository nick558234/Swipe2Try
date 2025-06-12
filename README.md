# Swipe2Try Architecture

This project follows a clean, layered architecture to maintain separation of concerns and improve maintainability. The application is divided into three main layers.

## 1. Core Layer (Swipe2Try.Core)

The Core layer is the innermost layer and has no dependencies on other project layers.

**Contents:**
- **Models**: Domain entities (`Dish`, `User`, `Role`)
- **Interfaces**: Contracts for repositories (`IDishRepository`, `IUserRepository`, `IRoleRepository`)
- **Validation**: Business rules and validation logic (`IUserValidator`)
- **Managers**: Core business logic interfaces and implementations (`IUserManager`, `IRoleManager`)

**Purpose:**
- Defines the domain model and business rules
- Contains interfaces that are implemented by outer layers
- Independent of implementation details (database, UI, etc.)

## 2. Data Access Layer (Swipe2Try.DAL)

The DAL layer depends on the Core layer but has no knowledge of the Presentation layer.

**Contents:**
- **Repositories**: Implementation of Core interfaces (`DishRepository`, `UserRepository`, `RoleRepository`)
- **Database-specific code**: Connection logic, SQL queries, etc.
- **Data mapping**: Conversion between database records and domain models

**Purpose:**
- Implements data access interfaces defined in Core
- Handles database operations and data mapping
- Isolates the application from database changes

## 3. Presentation Layer (Swipe2Try)

The Presentation layer depends on the Core layer directly, and uses implementations from the DAL layer via dependency injection.

**Contents:**
- **Pages**: Razor Pages UI (Admin, RestaurantOwner, etc.)
- **Components**: Reusable UI components
- **Program.cs**: Application startup and dependency injection

**Purpose:**
- Implements the user interface
- Handles user interactions
- Coordinates between views and repositories
- Manages authentication and authorization

## How the Layers Interact

1. The Presentation layer page model (e.g., `UpdateModel`) requests data from a repository interface
2. The interface is defined in the Core layer (e.g., `IUserRepository`)
3. The actual implementation is in the DAL layer (e.g., `UserRepository`)
4. The implementation interacts with the database and returns domain models from the Core layer
5. The Presentation layer displays the data to the user

### Authentication Flow Example

1. User submits login credentials through the Presentation layer
2. Credentials are validated using `UserValidator` from the Core layer
3. `UserManager` (Core) authenticates the user using `IUserRepository`
4. `UserRepository` (DAL) retrieves user data from the database
5. Authentication result is returned to the Presentation layer
6. User session is created upon successful authentication

For a detailed explanation of the authentication system, see the [Authentication Documentation](docs/Authentication.md).

## Current Dependency Flow

```
                                   ┌──────────────────────────┐
                                   │                          │
                                   │  Presentation Layer      │
                                   │  (Swipe2Try)             │
                                   │                          │
                                   │  - Razor Pages           │
                                   │  - Page Models           │
                                   │  - partial views         │
                                   │                          │
                                   └───────────┬──────────────┘
                                               │
                                 ┌─────────────┼─────────────┐
                                 │             │             │
                        ┌────────▼─────────┐   │    ┌────────▼─────────┐
                        │                  │   │    │                  │
                        │  Core Layer      │◄──┘    │  DAL Layer       │
                        │  (Swipe2Try.Core)│        │  (Swipe2Try.DAL) │
                        │                  │◄───────┤                  │
                        │  - Models        │        │  - Repositories  │
                        │  - Interfaces    │        │  - SQL Logic     │
                        │  - Validation    │        │  - Data Mapping  │
                        │  - Managers      │        │                  │
                        │                  │        │                  │
                        └──────────────────┘        └──────────────────┘
```

This diagram illustrates the clean architecture implementation where:

1. The Presentation Layer depends on both Core and DAL layers
2. The Core Layer has no dependencies on other layers
3. The DAL Layer depends only on the Core Layer

This dependency direction ensures that the business logic (Core) remains isolated from implementation details, while the data access components properly implement the interfaces defined in the Core layer.