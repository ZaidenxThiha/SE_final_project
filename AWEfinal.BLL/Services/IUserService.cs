using AWEfinal.DAL.Models;

namespace AWEfinal.BLL.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> RegisterUserAsync(string email, string password, string name);
        Task<User?> LoginUserAsync(string email, string password);
        Task<User> UpdateUserAsync(User user);
        Task<User> UpdateProfileAsync(int userId, string name, string? phone, string? addressLine1,
            string? addressLine2, string? city, string? postalCode, string? country);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> UserExistsAsync(string email);
    }
}
