# Swipe2Try Architecture

This project follows a clean, layered architecture to maintain separation of concerns and improve maintainability. The application is currently divided into three main layers with potential for further refinement.

## 1. Core Layer (Swipe2Try.Core)

The Core layer is the innermost layer and has no dependencies on other project layers.

**Contents:**
- **Models**: Domain entities that represent the business objects (e.g., `Dish`)
- **Interfaces**: Contracts that define operations (e.g., `IDishRepository`)

**Purpose:**
- Defines the domain model and business rules
- Contains interfaces that will be implemented by outer layers
- Independent of implementation details (database, UI, etc.)

**When to use:**
- When defining new business entities
- When creating contracts for data access or services

## 2. Data Access Layer (Swipe2Try.DAL)

The DAL layer depends on the Core layer but has no knowledge of the Presentation layer.

**Contents:**
- **Repositories**: Implementation of Core interfaces (e.g., `DishRepository`)
- **Database-specific code**: Connection logic, queries, etc.

**Purpose:**
- Implements data access interfaces defined in Core
- Handles database operations and data mapping
- Isolates the application from database changes

**When to use:**
- When implementing data access logic
- When querying or updating the database
- When mapping database results to domain models

## 3. Presentation Layer (Swipe2Try)

The Presentation layer depends on the Core layer directly, and uses implementations from the DAL layer via dependency injection.

**Contents:**
- **Pages**: Razor Pages UI (e.g., `Admin/Dishes/Index.cshtml`)
- **Components**: Reusable UI components
- **Program.cs**: Application startup and dependency injection

**Purpose:**
- Implements the user interface
- Handles user interactions
- Coordinates between views and repositories

**When to use:**
- When building user interfaces
- When handling user requests and displaying data
- When setting up application services and dependencies

## How the Layers Interact

1. The Presentation layer page model (e.g., `IndexModel`) requests data from a repository interface
2. The interface is defined in the Core layer (`IDishRepository`)
3. The actual implementation is in the DAL layer (`DishRepository`)
4. The implementation interacts with the database and returns domain models from the Core layer
5. The Presentation layer displays the data to the user

## Current Dependency Flow

```
Presentation Layer (Swipe2Try)
        │
        ├─────► Core Layer (Swipe2Try.Core)
        │          ▲
        │          │
        │          │
        └─────► DAL Layer (Swipe2Try.DAL)
```

## Future Architecture Improvements

For a more complete clean architecture, consider these potential enhancements:

1. **Add a Service/Application Layer**: Insert a layer between Presentation and DAL to handle business logic:
   ```
   Presentation → Application/Services → Core ← DAL
   ```

2. **Configuration Management**: Create an abstraction for configuration in the Core layer, with implementations in the infrastructure layer.

3. **Unit Tests**: Add test projects for each layer to ensure proper separation of concerns.

This architecture ensures the Core layer remains independent of implementation details, making the application more maintainable and testable. The current implementation provides a solid foundation that can be extended as the application grows in complexity. 