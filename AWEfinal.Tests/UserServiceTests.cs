using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using AWEfinal.DAL.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AWEfinal.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task Login_WithHashedPassword_Succeeds()
        {
            var hash = Sha256("p@ssw0rd");
            var user = new User { Id = 1, Email = "user@test.com", PasswordHash = hash, Role = "customer" };

            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

            var svc = new UserService(repo.Object);
            var result = await svc.LoginUserAsync("user@test.com", "p@ssw0rd");

            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        public async Task Login_AdminFallback_OnlyWhenNoHashAndPasswordMatches()
        {
            var admin = new User { Id = 1, Email = "admin@electrostore.com", PasswordHash = string.Empty, Role = "admin" };
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByEmailAsync("admin@electrostore.com")).ReturnsAsync(admin);

            var svc = new UserService(repo.Object);

            var ok = await svc.LoginUserAsync("admin@electrostore.com", "admin123");
            ok.Should().NotBeNull();

            var fail = await svc.LoginUserAsync("admin@electrostore.com", "wrong");
            fail.Should().BeNull();
        }

        [Fact]
        public async Task Login_WrongPassword_Fails()
        {
            var hash = Sha256("right");
            var user = new User { Id = 1, Email = "user@test.com", PasswordHash = hash, Role = "customer" };
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

            var svc = new UserService(repo.Object);
            var result = await svc.LoginUserAsync("user@test.com", "wrong");

            result.Should().BeNull();
        }

        [Fact]
        public async Task ChangePassword_BadCurrent_Throws()
        {
            var hash = Sha256("old");
            var user = new User { Id = 1, Email = "user@test.com", PasswordHash = hash, Role = "customer" };
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);

            var svc = new UserService(repo.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => svc.ChangePasswordAsync(1, "wrong", "newpass123"));
        }

        [Fact]
        public async Task ChangePassword_Valid_UpdatesHash()
        {
            var hash = Sha256("old");
            var user = new User { Id = 1, Email = "user@test.com", PasswordHash = hash, Role = "customer" };
            var repo = new Mock<IUserRepository>();
            repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            repo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

            var svc = new UserService(repo.Object);
            await svc.ChangePasswordAsync(1, "old", "newpass123");

            user.PasswordHash.Should().Be(Sha256("newpass123"));
        }

        private static string Sha256(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}
