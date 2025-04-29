using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InsightBoard.Api.Data;
using InsightBoard.Api.DTOs.Auth;
using InsightBoard.Api.Exceptions;
using InsightBoard.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LoginRequest = InsightBoard.Api.DTOs.Auth.LoginRequest;
using RegisterRequest = InsightBoard.Api.DTOs.Auth.RegisterRequest;

namespace InsightBoard.Api.Services.Auth;

public class AuthService : IAuthService
{
    private readonly InsightBoardDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public AuthService(InsightBoardDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new BadRequestException("User with this email already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return GenerateAuthResponse(user, refreshToken.Id);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            throw new UnauthorizedException("Wrong email or password.");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Wrong email or password.");
        }


        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return GenerateAuthResponse(user, refreshToken.Id);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(r => r.Id == request.RefreshToken);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }


        refreshToken.IsRevoked = true;

        var newRefreshToken = new RefreshToken
        {
            UserId = refreshToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return GenerateAuthResponse(refreshToken.User, newRefreshToken.Id);
    }

    private AuthResponse GenerateAuthResponse(User user, string refreshTokenId)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(15); 

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );
        
        Console.WriteLine("JWT key length: " + (_configuration["Jwt:Key"]?.Length ?? 0));

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = expires,
            RefreshToken = refreshTokenId
        };
    }
}
