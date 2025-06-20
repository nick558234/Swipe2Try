using Swipe2Try.Core.Models;

namespace Swipe2Try.Core.Interfaces;

public interface IUserDishPreferenceRepository
{
    Task<UserDishPreference?> GetUserDishPreferenceAsync(string userId, int dishId);
    Task<List<UserDishPreference>> GetUserPreferencesAsync(string userId);
    Task<List<UserDishPreference>> GetDishPreferencesAsync(int dishId);
    Task AddUserDishPreferenceAsync(UserDishPreference preference);
    Task UpdateUserDishPreferenceAsync(UserDishPreference preference);
    Task DeleteUserDishPreferenceAsync(string userId, int dishId);
}
