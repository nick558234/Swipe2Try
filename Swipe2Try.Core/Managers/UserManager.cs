using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.Core.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserValidator _userValidator;

        public UserManager(IUserRepository userRepository, IUserValidator userValidator)
        {
            _userRepository = userRepository;
            _userValidator = userValidator;
        }

        public async Task<(bool Success, List<string> Errors)> RegisterUserAsync(User user)
        {
            // Validate user input
            var (isValid, errors) = await _userValidator.ValidateForRegistrationAsync(user);
            if (!isValid)
                return (false, errors);

            // Generate a random UserID (10 characters)
            user.UserID = GenerateUserID();

            // Create user in database
            var success = await _userRepository.CreateUserAsync(user);
            return (success, success ? new List<string>() : new List<string> { "Failed to create user" });
        }

        public async Task<(bool Success, User User, List<string> Errors)> AuthenticateUserAsync(string email, string password)
        {
            var errors = new List<string>();
            
            // Validate login input
            var (isValid, validationErrors) = _userValidator.ValidateForLogin(email, password);
            if (!isValid)
                return (false, null, validationErrors);

            // Get user from database
            var user = await _userRepository.GetUserByEmailAsync(email);
            
            // Check if user exists and password matches
            if (user == null || user.Password != password)
            {
                errors.Add("Invalid email or password");
                return (false, null, errors);
            }

            return (true, user, errors);
        }

        private string GenerateUserID()
        {
            // Simple implementation for demo purposes - in production use a more secure method
            return Guid.NewGuid().ToString("N").Substring(0, 10);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
    }
} 