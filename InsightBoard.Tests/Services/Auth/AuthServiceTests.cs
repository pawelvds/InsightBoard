using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using InsightBoard.Api.Data;
using InsightBoard.Api.DTOs.Auth;
using InsightBoard.Api.Exceptions;
using InsightBoard.Api.Models;
using InsightBoard.Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace InsightBoard.Tests.Services.Auth
{
    public class AuthServiceTests
    {
        private readonly InsightBoardDbContext _context;
        private readonly Mock<IConfiguration> _configMock;
        private readonly TestAuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<InsightBoardDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InsightBoardDbContext(options);

            _configMock = new Mock<IConfiguration>();
            var configSectionMock = new Mock<IConfigurationSection>();
            configSectionMock.Setup(s => s.Value).Returns("test_secret_key_at_least_32_characters_long_for_testing");
            
            _configMock.Setup(c => c["Jwt:Key"]).Returns("test_secret_key_at_least_32_characters_long_for_testing");
            _configMock.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
            _configMock.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

            _authService = new TestAuthService(_context, _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailExists()
        {
            // Arrange
            var existingUser = new User
            {
                Email = "existing@example.com",
                Username = "existinguser"
            };

            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            var request = new RegisterRequest
            {
                Email = "existing@example.com",
                Username = "newuser",
                Password = "Password123!"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(
                () => _authService.RegisterAsync(request));

            Assert.Equal("User with this email already exists.", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser_WhenValidRequest()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "new@example.com",
                Username = "newuser",
                Password = "Password123!"
            };

            _authService.HashPasswordResult = "hashed_password";

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            Assert.NotNull(user);
            Assert.Equal(request.Username, user.Username);
            Assert.Equal("hashed_password", user.PasswordHash);

            var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == user.Id);
            Assert.NotNull(refreshToken);
            Assert.False(refreshToken.Used);
            Assert.False(refreshToken.Invalidated);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(request));

            Assert.Equal("Wrong email or password.", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenPasswordIsWrong()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = "hashed_password"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword123!"
            };

            _authService.VerifyPasswordResult = PasswordVerificationResult.Failed;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(request));

            Assert.Equal("Wrong email or password.", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Username = "testuser",
                PasswordHash = "hashed_password"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "ValidPassword123!"
            };

            _authService.VerifyPasswordResult = PasswordVerificationResult.Success;

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);

            var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(r => r.UserId == user.Id);
            Assert.NotNull(refreshToken);
            Assert.False(refreshToken.Used);
            Assert.False(refreshToken.Invalidated);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenTokenNotFound()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = "nonexistent-token"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.RefreshTokenAsync(request));

            Assert.Equal("Invalid refresh token.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenTokenIsUsed()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Username = "testuser"
            };

            _context.Users.Add(user);

            var token = new RefreshToken
            {
                Token = "used-token",
                JwtId = "jwt-id",
                UserId = user.Id,
                Used = true,
                Invalidated = false,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            var request = new RefreshTokenRequest
            {
                RefreshToken = "used-token"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.RefreshTokenAsync(request));

            Assert.Equal("Invalid refresh token.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowException_WhenTokenIsExpired()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Username = "testuser"
            };

            _context.Users.Add(user);

            var token = new RefreshToken
            {
                Token = "expired-token",
                JwtId = "jwt-id",
                UserId = user.Id,
                Used = false,
                Invalidated = false,
                CreationDate = DateTime.UtcNow.AddDays(-8),
                ExpiryDate = DateTime.UtcNow.AddDays(-1) 
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            var request = new RefreshTokenRequest
            {
                RefreshToken = "expired-token"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.RefreshTokenAsync(request));

            Assert.Equal("Invalid refresh token.", exception.Message);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenTokenIsValid()
        {
            // Arrange
            var user = new User
            {
                Email = "test@example.com",
                Username = "testuser"
            };

            _context.Users.Add(user);

            var token = new RefreshToken
            {
                Token = "valid-token",
                JwtId = "jwt-id",
                UserId = user.Id,
                Used = false,
                Invalidated = false,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            var request = new RefreshTokenRequest
            {
                RefreshToken = "valid-token"
            };

            // Act
            var result = await _authService.RefreshTokenAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.NotEqual("valid-token", result.RefreshToken);

            var oldToken = await _context.RefreshTokens.SingleOrDefaultAsync(r => r.Token == "valid-token");
            Assert.NotNull(oldToken);
            Assert.True(oldToken.Used);

            var newToken = await _context.RefreshTokens.SingleOrDefaultAsync(r => r.Token == result.RefreshToken);
            Assert.NotNull(newToken);
            Assert.False(newToken.Used);
            Assert.False(newToken.Invalidated);
            Assert.Equal(user.Id, newToken.UserId);
        }
    }

    public class TestAuthService : AuthService
    {
        public PasswordVerificationResult VerifyPasswordResult { get; set; } = PasswordVerificationResult.Failed;
        public string HashPasswordResult { get; set; } = "test_hash";
        public DateTime ExpirationResult { get; set; } = DateTime.UtcNow.AddMinutes(15);

        public TestAuthService(InsightBoardDbContext context, IConfiguration configuration)
            : base(context, configuration)
        {
        }

        protected override PasswordVerificationResult VerifyPassword(User user, string password)
        {
            return VerifyPasswordResult;
        }

        protected override string HashPassword(User user, string password)
        {
            return HashPasswordResult;
        }

        protected override JwtSecurityToken GenerateJwtToken(User user, out DateTime expires)
        {
            expires = ExpirationResult;
            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, "test-jwt-id")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test_secret_key_at_least_32_characters_long_for_testing"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                "test-issuer",
                "test-audience",
                claims,
                expires: expires,
                signingCredentials: creds
            );
        }
    }
}