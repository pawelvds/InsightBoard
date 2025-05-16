using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InsightBoard.Api.Data;
using InsightBoard.Api.DTOs.Auth;
using InsightBoard.Api.Exceptions;
using InsightBoard.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        
        var jwtToken = GenerateJwtToken(user, out DateTime expires);
        
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            JwtId = jwtToken.Id,
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            Used = false,
            Invalidated = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            Expiration = expires,
            RefreshToken = refreshToken.Token
        };
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

        var jwtToken = GenerateJwtToken(user, out DateTime expires);
        
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            JwtId = jwtToken.Id,
            UserId = user.Id,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            Used = false,
            Invalidated = false
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            Expiration = expires,
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(r => r.Token == request.RefreshToken);

        if (refreshToken == null || refreshToken.Used || refreshToken.Invalidated || refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        refreshToken.Used = true;
        
        var jwtToken = GenerateJwtToken(refreshToken.User, out DateTime expires);
        
        var newRefreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            JwtId = jwtToken.Id,
            UserId = refreshToken.UserId,
            CreationDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            Used = false,
            Invalidated = false
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            Expiration = expires,
            RefreshToken = newRefreshToken.Token
        };
    }

    private JwtSecurityToken GenerateJwtToken(User user, out DateTime expires)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        expires = DateTime.UtcNow.AddMinutes(15); 

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: expires,
            signingCredentials: creds
        );
        
        return token;
    }
}