# 🛡️ Authorization in Swipe2Try

This document outlines the authorization mechanism implemented in the Swipe2Try application, which now uses ASP.NET Core's built-in authorization system.

## 🧩 Core Components

### 1. ASP.NET Core Authentication/Authorization
📍 Configured in `Program.cs`.

The application uses ASP.NET Core's cookie-based authentication and built-in authorization:

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

### 2. 🔒 Role-Based Authorization with `[Authorize]` Attribute
The application uses role-based authorization by applying the `[Authorize]` attribute to Razor Pages:

| Page/Area                 | Required Role(s)        | Authorization Attribute                 |
|---------------------------|-------------------------|-----------------------------------------|
| `/Admin/`                 | "ADMIN"                 | `[Authorize(Roles = "ADMIN")]`          |
| `/RestaurantOwner/`       | "OWNER"                 | `[Authorize(Roles = "OWNER")]`          |
| `/Admin/Dishes/`          | "ADMIN" or "OWNER"      | `[Authorize(Roles = "ADMIN,OWNER")]`    |
| `/Account/Settings/`      | "ADMIN" or "OWNER"      | `[Authorize(Roles = "ADMIN,OWNER")]`    |
| `/Profile/`               | Any logged-in user      | `[Authorize]` (no roles specified)      |
| `/Account/`               | Any logged-in user      | `[Authorize]` (no roles specified)      |
| `/swipe`                  | Any logged-in user      | `[Authorize]` (no roles specified)      |

### 3. 🔑 Authentication with Claims (`Pages/login.cshtml.cs`)
- Upon successful authentication, the `loginModel` fetches the user's role and creates claims:

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, result.User.Name),
    new Claim(ClaimTypes.Email, result.User.Email),
    new Claim(ClaimTypes.Role, roleNameToStore)
};
var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
```

- After setting the claims, users are redirected based on their role:
    - "ADMIN" ➡️ `/Admin/Index`
    - "OWNER" ➡️ `/RestaurantOwner/Index`
    - Others ➡️ `/swipe`

### 4. 🚪 Logout Process (`Pages/logout.cshtml.cs`)
- The logout process uses ASP.NET Core's built-in sign-out mechanism:

```csharp
public async Task<IActionResult> OnGetAsync()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToPage("/Index");
}
```

### 5. ⚠️ Error Handling (`Pages/Error.cshtml` and `Pages/Error.cshtml.cs`)
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

## 👤 Accessing User Information in Razor Pages
With the ASP.NET Core Identity system, user information is now available through the `User` property:

```html
<div>
    @if (User.Identity?.IsAuthenticated == true)
    {
        <p>Welcome, @User.Identity?.Name!</p>
        <p>Your role: @User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value</p>
        
        @if (User.IsInRole("ADMIN"))
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

## 🔄 Role Considerations
- Role names are case-sensitive (e.g., "ADMIN" is different from "Admin")
- All role checks in the application use uppercase role names for consistency

This setup leverages ASP.NET Core's built-in authentication and authorization features, providing a secure and maintainable approach to protecting sensitive areas of the application.
