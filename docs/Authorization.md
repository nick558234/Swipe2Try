# 🛡️ Authorization in Swipe2Try

This document outlines the authorization mechanism implemented in the Swipe2Try application using ASP.NET Core's built-in authorization system.

## 📊 Diagrams

For a visual representation of the authorization system, refer to:

- [Authentication Class Diagram](diagrams/auth_class_diagram.mmd): Displays the relationships between classes involved in authentication and authorization
- [User Role Relationships](diagrams/user_role_relationships.mmd): Shows how users are assigned roles and permissions
- [Three-Layer Architecture](diagrams/three_layer_architecture.mmd): Overall system architecture including security layers

## 🧩 Core Components

### 1. ASP.NET Core Authentication/Authorization
📍 Configured in `Program.cs`.

Swipe2Try utilizes ASP.NET Core's built-in authorization attributes and middleware:

```csharp
// Add built-in cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/Error?code=403";
    });

// Add standard authorization
builder.Services.AddAuthorization();
```

- **`[Authorize]` Attribute**: Applied to controller actions or Razor Pages to restrict access.
- **Role-Based Authorization**: Implemented using `[Authorize(Roles = "RoleName")]` attribute.
- **Authentication Cookie**: Contains user identity and role claims.

Example of role-based authorization on a page:
```csharp
[Authorize(Roles = "Admin,OWNER")]
public class IndexModel : PageModel
{
    // Page model implementation
}
```

### 2. 🔒 Role-Based Authorization with `[Authorize]` Attribute
The application uses role-based authorization by applying the `[Authorize]` attribute to Razor Pages:

| Page/Area                 | Required Role(s)        | Authorization Attribute                 |
|---------------------------|-------------------------|-----------------------------------------|
| `/Admin/`                 | "Admin"                 | `[Authorize(Roles = "Admin")]`          |
| `/RestaurantOwner/`       | "OWNER"                 | `[Authorize(Roles = "OWNER")]`          |
| `/Admin/Dishes/`          | "Admin" or "OWNER"      | `[Authorize(Roles = "Admin,OWNER")]`    |
| `/Account/Settings/`      | "Admin" or "OWNER"      | `[Authorize(Roles = "Admin,OWNER")]`    |
| `/Profile/`               | Any logged-in user      | `[Authorize]` (no roles specified)      |
| `/Account/`               | Any logged-in user      | `[Authorize]` (no roles specified)      |
| `/swipe`                  | Any logged-in user      | `[Authorize]` (no roles specified)      |

### 3. 🔑 Claims-Based Identity
User information and roles are stored as claims in the authentication cookie:

- **`ClaimTypes.Name`**: Stores the user's name.
- **`ClaimTypes.Email`**: Stores the user's email address.
- **`ClaimTypes.Role`**: Stores the role name (e.g., "Admin", "OWNER", "User").

These claims are created during login and stored in the authentication cookie:

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, result.User.Name),
    new Claim(ClaimTypes.Email, result.User.Email),
    new Claim(ClaimTypes.Role, roleNameToStore)
};
var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
```

After setting the claims, users are redirected based on their role:
- "Admin" ➡️ `/Admin/Index`
- "OWNER" ➡️ `/RestaurantOwner/Index`
- Others ➡️ `/swipe`

### 4. 🚀 Role Management
Roles are stored in the database and retrieved through the `IRoleManager` service:

- Each user has a `RoleID` property that references a role.
- During authentication, the role name is fetched from the database.
- The role name is then stored as a claim in the authentication cookie.

### 5. ⚠️ Error Handling for Authorization
When a user attempts to access a resource they're not authorized for:

- The user is redirected to the path specified in `options.AccessDeniedPath` ("/Error?code=403").
- The error page can display a specific message for unauthorized access.
- Shows a specific message for 403 (Forbidden) errors when access is denied
- Hides detailed development information for security reasons

## 🌊 Authorization Flow Diagram

```mermaid
graph TD
    A[User Request] --> B{Is Page Protected with [Authorize]?}
    B -- Yes --> C{User Authenticated?}
    B -- No --> G[Allow Access]
    C -- No --> D[Redirect to /login]
    C -- Yes --> E{User Has Required Role?}
    E -- Yes --> G
    E -- No --> F[Redirect to /Error?code=403]
```

## 👥 Access Control by Role

The application implements role-based access control with these primary roles:

| Role        | Access Areas                                   | Capabilities                                 |
|-------------|------------------------------------------------|----------------------------------------------|
| **Admin**   | `/Admin/*`, All areas                          | Full system administration                   |
| **OWNER**   | `/RestaurantOwner/*`, Limited admin functions  | Manage restaurant dishes and settings        |
| **User**    | `/swipe/*`, `/profile/*`, `/account/*`         | Basic user functionality                     |

## 👤 Accessing User Information in Razor Pages
User information is available through the `User` property in Razor Pages:

```html
<div>
    @if (User.Identity?.IsAuthenticated == true)
    {
        <p>Welcome, @User.Identity?.Name!</p>
        <p>Your role: @User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value</p>
        
        @if (User.IsInRole("Admin"))
        {
            <p>You have administrator access.</p>
        }
    }
    else
    {
        <p>Please <a href="/login">sign in</a>.</p>
    }
</div>
```

You can also access user information in page models:

```csharp
// Get user information from claims
UserName = User.Identity?.Name ?? "Guest";
UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
```

The UI adapts based on the user's role, providing different colors and navigation options.

## 🔓 Logout Functionality

The application provides a logout mechanism through:

1. A logout button in the UI that links to the `/logout` endpoint.
2. The logout endpoint that calls `SignOutAsync` to clear the authentication cookie:

```csharp
public async Task<IActionResult> OnGetAsync()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToPage("/Index");
}
```

## 🔄 Role Considerations
- Role names are case-sensitive (e.g., "admin" is different from "Admin")
- All role checks in the application use consistent role names for reliability

This comprehensive authorization system leverages ASP.NET Core's built-in authentication and authorization features, providing a secure and maintainable approach to protecting sensitive areas of the application.
