# 🛡️ Authorization in Swipe2Try

This document outlines the authorization mechanism implemented in the Swipe2Try application.

## 🧩 Core Components

### 1. `AuthorizationMiddleware.cs`
📍 Located in `Swipe2Try/Middleware/AuthorizationMiddleware.cs`.

This middleware is responsible for intercepting requests and performing authorization checks.
- ⚙️ Runs for every request after authentication.
- 🔍 Checks if the requested path requires authorization.

Protected paths and required roles:

| Path Prefix         | Required Role(s)        | Notes                                     |
|---------------------|-------------------------|-------------------------------------------|
| `/admin/`           | "ADMIN"                 | Case-insensitive                          |
| `/restaurantowner/` | "OWNER" or "ADMIN"      | Case-insensitive                          |
| `/profile/`         | Any logged-in user      | Session `UserRole` must exist             |
| `/account/`         | Any logged-in user      | Session `UserRole` must exist             |
| `/swipe` & `/swipe/*` | Any logged-in user      | Session `UserRole` must exist             |

- **🔑 Session Check**: It first checks if the user is logged in by looking for a `UserRole` in the session.
    - If `UserRole` is not found (user is not logged in), the user is redirected to `/login`.
- **🚫 Unauthorized Access**: If a user is logged in but does not have the required role for a specific path, they are redirected to `/Error?code=403` (Forbidden).

### 2. 📦 Session Variables
The following session variables are used for authorization and user information:
- `UserID`: Stores the unique identifier of the logged-in user.
- `UserName`: Stores the name of the logged-in user.
- `UserRole`: Stores the **name** of the role assigned to the logged-in user (e.g., "ADMIN", "OWNER", "User"). This is crucial for the middleware checks.

### 3. 🔑 Login Logic (`Pages/login.cshtml.cs`)
- Upon successful authentication, the `loginModel` fetches the user's `RoleID`.
- It then uses `IRoleManager` to look up the `RoleName` corresponding to the `RoleID`.
- The `RoleName` (e.g., "ADMIN") is then stored in `HttpContext.Session.SetString("UserRole", roleName);`.
- After setting the session variables, users are redirected based on their `UserRole`:
    - "ADMIN" ➡️ `/Admin/Index`
    - "OWNER" ➡️ `/RestaurantOwner/Index`
    - Others ➡️ `/swipe`

### 4. ⚠️ Error Handling (`Pages/Error.cshtml` and `Pages/Error.cshtml.cs`)
- The error page displays a specific message ("You are not authorized to access this page.") when the `code=403` query parameter is present.
- Detailed development-mode error information is suppressed for 403 errors.

## 🌊 Authorization Flow Diagram

```mermaid
graph TD
    A[User Request] --> B{Is Path Protected?};
    B -- Yes --> C{User Logged In? (Session UserRole Exists?)};
    B -- No --> G[Allow Access];
    C -- No --> D[Redirect to /login];
    C -- Yes --> E{User Has Required Role?};
    E -- Yes --> G;
    E -- No --> F[Redirect to /Error?code=403];
```

## 👤 User Information Display (`Pages/Shared/_UserInfoPartial.cshtml`)
- This partial view is displayed in the layout when a user is logged in.
- It retrieves `UserName` and `UserRole` from the session to display the user's name and their role.

```html
@using Microsoft.AspNetCore.Http
@inject IHttpContextAccessor HttpContextAccessor

@{
    var userName = HttpContextAccessor.HttpContext?.Session.GetString("UserName") ?? "Guest";
    var userRole = HttpContextAccessor.HttpContext?.Session.GetString("UserRole") ?? "Unknown";
    var initial = userName.Length > 0 ? userName[0] : 'G';
}

<div class="flex items-center p-2 rounded-lg hover:bg-gray-700 transition-colors duration-150">
    <div class="relative mr-3">
        <div class="flex items-center justify-center h-10 w-10 rounded-full bg-indigo-500 text-white font-semibold text-lg ring-2 ring-offset-2 ring-offset-gray-800 ring-indigo-400">
            @initial
        </div>
    </div>
    <div>
        <p class="text-sm font-semibold text-gray-100 group-hover:text-white">@userName</p>
        <p class="text-xs text-indigo-300 group-hover:text-indigo-200">@userRole</p>
    </div>
</div>
```

This setup ensures that sensitive areas of the application are protected and access is granted based on defined user roles stored by name in the session.
