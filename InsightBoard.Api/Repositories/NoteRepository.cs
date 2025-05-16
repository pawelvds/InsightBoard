using InsightBoard.Api.Data;
using InsightBoard.Api.Exceptions;
using InsightBoard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InsightBoard.Api.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly InsightBoardDbContext _context;
    private readonly IUserRepository _userRepository;

    public NoteRepository(InsightBoardDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<Note>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Notes
            .Include(n => n.User)
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Note?> GetByIdAsync(Guid id)
    {
        return await _context.Notes
            .Include(n => n.User)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task CreateAsync(Note note)
    {
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Note note)
    {
        _context.Notes.Update(note);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Note note)
    {
        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Note>> GetPublicNotesByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null)
            throw new NotFoundException("User not found.");

        return await _context.Notes
            .Include(n => n.User)
            .Where(n => n.IsPublic && n.UserId == user.Id)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Note>> GetPublicNotesAsync()
    {
        return await _context.Notes
            .Include(n => n.User)
            .Where(n => n.IsPublic)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<(IEnumerable<Note>, int)> GetPublicNotesPagedAsync(int pageNumber, int pageSize, string? sortBy)
    {
        var query = _context.Notes
            .Include(n => n.User)
            .Where(n => n.IsPublic);

        query = sortBy switch
        {
            "created_at" => query.OrderByDescending(n => n.CreatedAt),
            "created_at_desc" => query.OrderByDescending(n => n.CreatedAt),
            _ => query.OrderByDescending(n => n.CreatedAt)
        };
        
        var totalRecords = await query.CountAsync();
        
        var notes = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (notes, totalRecords);
    }
}