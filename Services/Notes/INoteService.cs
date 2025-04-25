using InsightBoard.Api.DTOs.Notes;

namespace InsightBoard.Api.Services.Notes;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllAsync();
    Task<NoteDto> CreateAsync(CreateNoteRequest request);
}