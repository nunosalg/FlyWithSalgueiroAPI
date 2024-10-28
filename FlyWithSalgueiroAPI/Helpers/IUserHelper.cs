using FlyWithSalgueiroAPI.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace FlyWithSalgueiroAPI.Helpers
{
    public interface IUserHelper
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<bool> CheckPasswordAsync(User user, string password);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<object?> GetUserImageAsync(string email);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    }
}
