using InsightBoard.Api.Models;

namespace InsightBoard.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByUsernameAsync(string username);
}