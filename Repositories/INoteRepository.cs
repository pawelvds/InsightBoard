using InsightBoard.Api.Models;

namespace InsightBoard.Api.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllByUserIdAsync(string userId);
    Task<Note?> GetByIdAsync(Guid id);
    Task CreateAsync(Note note);
    Task UpdateAsync(Note note);
    Task DeleteAsync(Note note);
    Task<IEnumerable<Note>> GetPublicNotesByUsernameAsync(string username);
    Task<IEnumerable<Note>> GetPublicNotesAsync();
    Task<(IEnumerable<Note>, int)> GetPublicNotesPagedAsync(int pageNumber, int pageSize, string? sortBy);
}