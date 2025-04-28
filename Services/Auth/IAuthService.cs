using InsightBoard.Api.DTOs.Auth;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = InsightBoard.Api.DTOs.Auth.LoginRequest;
using RegisterRequest = InsightBoard.Api.DTOs.Auth.RegisterRequest;

namespace InsightBoard.Api.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request); 
}