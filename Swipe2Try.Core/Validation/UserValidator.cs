using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Swipe2Try.Core.Validation
{
    public class UserValidator : IUserValidator
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserValidator(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForRegistrationAsync(User user)
        {
            var errors = new List<string>();

            // Check required fields
            if (string.IsNullOrWhiteSpace(user.Name))
                errors.Add("Name is required");

            if (string.IsNullOrWhiteSpace(user.Email))
                errors.Add("Email is required");
            else if (!IsValidEmail(user.Email))
                errors.Add("Invalid email format");
            else if (await _userRepository.EmailExistsAsync(user.Email))
                errors.Add("Email is already registered");

            if (string.IsNullOrWhiteSpace(user.Password))
                errors.Add("Password is required");
            else if (user.Password.Length < 6)
                errors.Add("Password must be at least 6 characters");

            if (string.IsNullOrWhiteSpace(user.RoleID))
                errors.Add("Role must be selected");
            else if (!await _roleRepository.RoleExistsAsync(user.RoleID))
                errors.Add("Selected role is not valid");

            return (errors.Count == 0, errors);
        }

        public (bool IsValid, List<string> Errors) ValidateForLogin(string email, string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email is required");
            else if (!IsValidEmail(email))
                errors.Add("Invalid email format");

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Password is required");

            return (errors.Count == 0, errors);
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
} 