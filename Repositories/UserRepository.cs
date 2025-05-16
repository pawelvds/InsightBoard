using InsightBoard.Api.Data;
using InsightBoard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InsightBoard.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly InsightBoardDbContext _context;

    public UserRepository(InsightBoardDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}