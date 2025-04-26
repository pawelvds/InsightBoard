using InsightBoard.Api.DTOs.Auth;

namespace InsightBoard.Api.Services.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}