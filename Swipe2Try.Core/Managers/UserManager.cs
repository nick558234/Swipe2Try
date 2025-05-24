using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Security.Claims;

namespace Swipe2Try.Core.Managers
{
    public class UserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserValidator _userValidator;
        private readonly RoleManager _roleManager;

        public UserManager(IUserRepository userRepository, IUserValidator userValidator, RoleManager roleManager)
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
            _roleManager = roleManager;
        }        public async Task<(bool Success, List<string> Errors)> RegisterUserAsync(User user)
        {
            // Validate user input
            var (isValid, errors) = await _userValidator.ValidateForRegistrationAsync(user);
            if (!isValid)
                return (false, errors);

            // Generate a unique UserID using full GUID
            user.UserID = GenerateUserID();

            // Create user in database
            var success = await _userRepository.CreateUserAsync(user);
            return (success, success ? new List<string>() : new List<string> { "Failed to create user" });
        }

        public async Task<(bool Success, User? User, List<string> Errors)> AuthenticateUserAsync(string email, string password)
        {
            var errors = new List<string>();

            // Validate login input
            var (isValid, validationErrors) = _userValidator.ValidateForLogin(email, password);
            if (!isValid)
                return (false, null, validationErrors);

            // Get user from database (case-insensitive email)
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                errors.Add("Invalid email or password");
                return (false, null, errors);
            }

            // Compare emails case-insensitively and passwords as plain text
            if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase) || user.Password != password)
            {
                errors.Add("Invalid email or password");
                return (false, null, errors);
            }

            return (true, user, errors);
        }        private string GenerateUserID()
        {
            // Generate full GUID for 40-character UserID column
            return Guid.NewGuid().ToString();
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<(bool Success, List<string> Errors, ClaimsPrincipal? Principal)> LoginUserAsync(string email, string password)
        {
            // Validate inputs using the UserValidator
            var validationResult = _userValidator.ValidateForLogin(email, password);
            if (!validationResult.IsValid)
            {
                return (false, validationResult.Errors, null);
            }

            // Authenticate user
            var authResult = await AuthenticateUserAsync(email, password);
            if (!authResult.Success || authResult.User == null)
            {
                return (false, authResult.Errors ?? new List<string> { "Invalid login attempt." }, null);
            }

            // Get role from database
            var role = await _roleManager.GetRoleByIdAsync(authResult.User.RoleID);
            var roleNameToStore = role?.RoleName ?? "Unknown";

            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authResult.User.Name),
                new Claim(ClaimTypes.Email, authResult.User.Email),
                new Claim(ClaimTypes.Role, roleNameToStore)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(claimsIdentity);

            return (true, new List<string>(), principal);
        }        public async Task<(bool Success, List<string> Errors, ClaimsPrincipal? Principal)> RegisterAndLoginUserAsync(string name, string email, string password, string roleId)
        {
            // Create user object and generate unique UserID
            var user = new User
            {
                UserID = GenerateUserID(),
                Name = name,
                Email = email,
                Password = password,
                RoleID = roleId
            };

            // Validate the user using the UserValidator
            var validationResult = await _userValidator.ValidateForRegistrationAsync(user);
            if (!validationResult.IsValid)
            {
                return (false, validationResult.Errors, null);
            }

            // Register user
            var registerResult = await RegisterUserAsync(user);
            if (!registerResult.Success)
            {
                return (false, registerResult.Errors, null);
            }

            // Get role name for claims
            var role = await _roleManager.GetRoleByIdAsync(user.RoleID);
            var roleName = role?.RoleName ?? "Unknown";

            // Create claims for immediate login
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(claimsIdentity);

            return (true, new List<string>(), principal);
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleManager.GetAllRolesAsync();
        }

        public string GetRedirectPageForRole(string roleName)
        {
            return roleName switch
            {
                "Admin" => "/Admin/Index",
                "OWNER" => "/RestaurantOwner/Index",
                _ => "/swipe"
            };
        }
    }
}