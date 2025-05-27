# Domain Models Documentation

This document provides comprehensive documentation of the core domain entities in the Swipe2Try application, including their properties, relationships, and business rules.

## Overview

The Swipe2Try domain model consists of four core entities that represent the fundamental business concepts:

- **User** - Represents system users with different roles
- **Role** - Defines user permissions and access levels  
- **Dish** - Represents food items managed by restaurant owners
- **Restaurant** - Represents restaurant establishments owned by users
- **Category** - Organizes dishes into logical groupings

## Core Entities

### User Entity

The `User` class represents all users in the system, regardless of their role.

```csharp
public class User
{
    public string UserID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RoleID { get; set; } = string.Empty;
}
```

#### Properties
- **UserID** (string, PK): Unique identifier for the user (max 10 chars)
- **Name** (string): Full name of the user (max 100 chars)
- **Email** (string): Email address used for login (max 255 chars, must be unique)
- **Password** (string): Hashed password (max 255 chars)
- **RoleID** (string, FK): Foreign key reference to Role entity

#### Business Rules
- Email must be unique across the system
- Password must be at least 6 characters long
- Email must follow valid email format
- Name and Email are required fields
- RoleID must reference an existing Role

#### Relationships
- **One-to-One** with Role (User belongs to one Role)
- **One-to-Many** with Restaurant (User can own multiple restaurants)
- **One-to-Many** with Dish (User can create multiple dishes)

---

### Role Entity

The `Role` class defines the different user types and their permissions.

```csharp
public class Role
{
    public string RoleID { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    
    public Role() { }
    
    public Role(string roleID, string roleName)
    {
        RoleID = roleID;
        RoleName = roleName;
    }
}
```

#### Properties
- **RoleID** (string, PK): Unique identifier for the role (max 10 chars)
- **RoleName** (string): Display name of the role (max 50 chars)

#### Standard Roles
- **Admin**: Full system administration privileges
- **Restaurant Owner**: Can manage restaurants and dishes
- **Customer**: Can browse and interact with dishes

#### Business Rules
- RoleName must be unique
- RoleID and RoleName are required fields
- Cannot delete roles that are assigned to users

#### Relationships
- **One-to-Many** with User (Role can be assigned to multiple users)

---

### Dish Entity

The `Dish` class represents food items that can be swiped by customers.

```csharp
public class Dish
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? HealthFactor { get; set; }
    public string? Photo { get; set; }
    public string? Restaurant { get; set; }
}
```

#### Properties
- **Id** (int, PK): Auto-generated unique identifier
- **Name** (string): Name of the dish (max 100 chars, required)
- **Description** (string): Detailed description (max 500 chars, required)
- **UserId** (string, FK): Foreign key to User who created the dish
- **HealthFactor** (int?, optional): Health rating from 1-10
- **Photo** (string?, optional): URL to dish image (max 500 chars)
- **Restaurant** (string?, optional): Restaurant name or identifier

#### Business Rules
- Name and Description are mandatory
- Name cannot exceed 100 characters
- Description cannot exceed 500 characters
- HealthFactor, if provided, must be between 1 and 10
- Photo URL, if provided, cannot exceed 500 characters
- Only the creating user can modify their dishes
- UserId must reference an existing User

#### Validation Rules
- **Creation**: Name, Description, and UserId are required
- **Update**: Same as creation plus valid Id must be provided
- **Security**: Users can only modify dishes they created

#### Relationships
- **Many-to-One** with User (Dish belongs to one User)
- **Many-to-One** with Restaurant (Dish belongs to one Restaurant)
- **Many-to-One** with Category (Dish belongs to one Category)

---

### Restaurant Entity

The `Restaurant` class represents restaurant establishments in the system.

```csharp
public class Restaurant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required string UserId { get; set; }
}
```

#### Properties
- **Id** (int, PK): Auto-generated unique identifier
- **Name** (string): Restaurant name (max 100 chars, required)
- **Location** (string): Restaurant address/location (max 200 chars, required)
- **UserId** (string, FK): Foreign key to User who owns the restaurant

#### Business Rules
- Name and Location are mandatory
- Name cannot exceed 100 characters
- Location cannot exceed 200 characters
- UserId cannot exceed 450 characters (ASP.NET Identity default)
- Only the owning user can modify their restaurants
- UserId must reference an existing User

#### Validation Rules
- **Creation**: Name, Location, and UserId are required
- **Update**: Same as creation plus valid Id must be provided
- **Security**: Users can only modify restaurants they own

#### Relationships
- **Many-to-One** with User (Restaurant belongs to one User/Owner)
- **One-to-Many** with Dish (Restaurant can have multiple dishes)

---

### Category Entity

The `Category` class organizes dishes into logical groupings.

```csharp
public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Photo { get; set; }
}
```

#### Properties
- **Id** (int, PK): Auto-generated unique identifier
- **Name** (string): Category name (max 100 chars, required)
- **Photo** (string?, optional): URL to category image (max 500 chars)

#### Business Rules
- Name is mandatory and must be unique
- Name cannot exceed 100 characters
- Photo URL, if provided, cannot exceed 500 characters
- Photo URL must be a valid absolute URL format

#### Validation Rules
- **Creation**: Name is required
- **Update**: Same as creation plus valid Id must be provided
- **URL Validation**: Photo URL must be well-formed if provided

#### Relationships
- **One-to-Many** with Dish (Category can contain multiple dishes)

---

## Entity Relationships Diagram

```
User (1) ----< (M) Restaurant
User (1) ----< (M) Dish
User (M) >---- (1) Role

Restaurant (1) ----< (M) Dish
Category (1) ----< (M) Dish
```

## Business Logic Patterns

### Tuple Return Pattern
All business operations use a consistent tuple return pattern for error handling:

```csharp
// Success/Error tuple pattern
Task<(bool Success, List<string> Errors)> AddEntityAsync(Entity entity)
Task<(bool Success, string Message)> AddEntityWithMessageAsync(Entity entity)
```

### Validation Pattern
Each entity has corresponding validation interfaces and implementations:

- `IUserValidator` / `UserValidator`
- `IDishValidator` / `DishValidator`
- `IRestaurantValidator` / `RestaurantValidator`
- `ICategoryValidator` / `CategoryValidator`

### Repository Pattern
Each entity has corresponding repository interfaces for data access:

- `IUserRepository` / `UserRepository`
- `IDishRepository` / `DishRepository`
- `IRestaurantRepository` / `RestaurantRepository`
- `ICategoryRepository` / `CategoryRepository`

### Manager Pattern
Business logic is encapsulated in manager classes:

- `UserManager` - User business operations
- `DishManager` - Dish business operations
- `RestaurantManager` - Restaurant business operations
- `CategoryManager` - Category business operations

## Security Considerations

### User Authorization
- Users can only modify entities they own (dishes, restaurants)
- Role-based access control determines UI access
- UserID validation ensures ownership before operations

### Data Validation
- All user input is validated at multiple layers
- SQL injection prevention through parameterized queries
- Cross-site scripting prevention through proper encoding

### Business Rule Enforcement
- Entity relationships are enforced at the business logic layer
- Cascade operations handle related entity cleanup
- Transaction management ensures data consistency

## Database Schema Alignment

The domain models map directly to the following database tables:

- **User** → `USERS` table
- **Role** → `ROLES` table  
- **Dish** → `DISHES` table
- **Restaurant** → `Restaurants` table
- **Category** → `Categories` table

Additional relationship tables:
- `LIKES_DISLIKES` - User dish interactions
- `DISHRESTAURANT` - Dish-Restaurant relationships
- `DISH_CATEGORIES` - Dish-Category relationships

## Usage Examples

### Creating a New Dish
```csharp
var dish = new Dish
{
    Name = "Margherita Pizza",
    Description = "Classic Italian pizza with tomato, mozzarella, and basil",
    UserId = currentUserId,
    HealthFactor = 6,
    Photo = "https://example.com/pizza.jpg"
};

var result = await dishManager.AddDishAsync(dish);
if (result.Success)
{
    // Dish created successfully
}
else
{
    // Handle validation errors in result.Errors
}
```

### Restaurant Ownership Validation
```csharp
var restaurant = await restaurantManager.GetRestaurantByIdAsync(restaurantId);
if (restaurant?.UserId != currentUserId)
{
    // User doesn't own this restaurant - unauthorized
    return Forbid();
}
```

This domain model design ensures data integrity, security, and maintainability while supporting the core business requirements of the Swipe2Try application.
