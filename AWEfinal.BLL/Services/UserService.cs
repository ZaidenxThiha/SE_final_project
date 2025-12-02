using AWEfinal.DAL.Models;
using AWEfinal.DAL.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace AWEfinal.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> RegisterUserAsync(string email, string password, string name)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required", nameof(password));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            if (await _userRepository.ExistsAsync(email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                Email = email.ToLower().Trim(),
                PasswordHash = HashPassword(password),
                Name = name.Trim(),
                Role = "customer",
                CreatedAt = DateTime.UtcNow
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<User?> LoginUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var normalizedEmail = email.ToLower().Trim();
            var user = await _userRepository.GetByEmailAsync(normalizedEmail);
            if (user == null)
                return null;

            // Verify via stored hash for all users/roles
            if (!string.IsNullOrWhiteSpace(user.PasswordHash) && VerifyPassword(password, user.PasswordHash))
                return user;

            // Admin fallback: allow default credentials only when no hash is stored
            if (string.IsNullOrWhiteSpace(user.PasswordHash)
                && string.Equals(normalizedEmail, "admin@electrostore.com", StringComparison.Ordinal)
                && string.Equals(password, "admin123", StringComparison.Ordinal))
            {
                return user;
            }

            return null;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!await _userRepository.ExistsAsync(user.Email))
                throw new ArgumentException("User not found", nameof(user));

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<User> UpdateProfileAsync(int userId, string name, string? phone, string? addressLine1,
            string? addressLine2, string? city, string? postalCode, string? country)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found", nameof(userId));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required", nameof(name));

            user.Name = name.Trim();
            user.Phone = phone?.Trim();
            user.AddressLine1 = addressLine1?.Trim();
            user.AddressLine2 = addressLine2?.Trim();
            user.City = city?.Trim();
            user.PostalCode = postalCode?.Trim();
            user.Country = country?.Trim();

            return await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password is required", nameof(newPassword));

            var user = await _userRepository.GetByIdAsync(userId) ?? throw new ArgumentException("User not found", nameof(userId));

            if (string.IsNullOrWhiteSpace(currentPassword) || !VerifyPassword(currentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _userRepository.ExistsAsync(email.ToLower().Trim());
        }

        private string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == passwordHash;
        }
    }
}
