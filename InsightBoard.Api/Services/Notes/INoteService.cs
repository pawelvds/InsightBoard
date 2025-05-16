using InsightBoard.Api.DTOs.Common;
using InsightBoard.Api.DTOs.Notes;

namespace InsightBoard.Api.Services.Notes;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllByUserIdAsync(string userId);
    Task<NoteDto> CreateAsync(CreateNoteRequest request, string userId);
    Task UpdateNoteAsync(string id, UpdateNoteRequest request, string userId);
    Task DeleteNoteAsync(string id, string userId);
    Task<IEnumerable<NoteDto>> GetPublicNotesAsync();
    Task PublishNoteAsync(string noteId, string userId);
    Task UnpublishNoteAsync(string noteId, string userId);
    Task<PagedResponse<NoteDto>> GetPublicNotesPagedAsync(int pageNumber, int pageSize, string? sortBy);
    Task<IEnumerable<NoteDto>> GetPublicNotesByUsernameAsync(string username);
    Task SetNotePublicStatusAsync(string id, string userId, bool isPublic);
}