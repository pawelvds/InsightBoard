using InsightBoard.Api.DTOs.Notes;

namespace InsightBoard.Api.Services.Notes;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllByUserIdAsync(string userId);
    Task<NoteDto> CreateAsync(CreateNoteRequest request, string userId);
}