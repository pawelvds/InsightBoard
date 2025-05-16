using AutoMapper;
using InsightBoard.Api.DTOs.Common;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Exceptions;
using InsightBoard.Api.Models;
using InsightBoard.Api.Repositories;

namespace InsightBoard.Api.Services.Notes;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;

    public NoteService(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<NoteDto>> GetAllByUserIdAsync(string userId)
    {
        var notes = await _noteRepository.GetAllByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<NoteDto>>(notes);
    }

    public async Task<NoteDto> CreateAsync(CreateNoteRequest request, string userId)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
            AuthorId = userId,
        };
        
        await _noteRepository.CreateAsync(note);
        return _mapper.Map<NoteDto>(note);
    }
    
    public async Task UpdateNoteAsync(string id, UpdateNoteRequest request, string userId)
    {
        if (!Guid.TryParse(id, out var noteGuid))
        {
            throw new BadRequestException("Invalid note ID.");
        }

        var note = await _noteRepository.GetByIdAsync(noteGuid);
        
        if (note == null)
        {
            throw new NotFoundException("Note not found.");
        }
        
        if (note.AuthorId != userId)
        {
            throw new UnauthorizedException("You are not authorized to update this note.");
        }

        note.Title = request.Title;
        note.Content = request.Content;

        await _noteRepository.UpdateAsync(note);
    }
    
    public async Task DeleteNoteAsync(string id, string userId)
    {
        if (!Guid.TryParse(id, out var noteGuid))
        {
            throw new BadRequestException("Invalid note ID.");
        }

        var note = await _noteRepository.GetByIdAsync(noteGuid);
        
        if (note == null)
        {
            throw new NotFoundException("Note not found.");
        }
        
        if (note.AuthorId != userId)
        {
            throw new UnauthorizedException("You are not authorized to delete this note.");
        }

        await _noteRepository.DeleteAsync(note);
    }

    public async Task<IEnumerable<NoteDto>> GetPublicNotesAsync()
    {
        var publicNotes = await _noteRepository.GetPublicNotesAsync();
        return _mapper.Map<IEnumerable<NoteDto>>(publicNotes);
    }

    public async Task PublishNoteAsync(string noteId, string userId)
    {
        if (!Guid.TryParse(noteId, out var noteGuid))
        {
            throw new BadRequestException("Invalid note ID.");
        }
        
        var note = await _noteRepository.GetByIdAsync(noteGuid);
        
        if (note == null)
            throw new NotFoundException("Note not found");
        
        if (note.AuthorId != userId)
            throw new UnauthorizedException("You are not authorized to publish this note");
        
        note.IsPublic = true;
        await _noteRepository.UpdateAsync(note);
    }

    public async Task UnpublishNoteAsync(string noteId, string userId)
    {
        if (!Guid.TryParse(noteId, out var noteGuid))
        {
            throw new BadRequestException("Invalid note ID.");
        }
        
        var note = await _noteRepository.GetByIdAsync(noteGuid);
        
        if (note == null)
            throw new NotFoundException("Note not found");
        
        if (note.AuthorId != userId)
            throw new UnauthorizedException("You are not authorized to unpublish this note");
        
        note.IsPublic = false;
        await _noteRepository.UpdateAsync(note);
    }

    public async Task<PagedResponse<NoteDto>> GetPublicNotesPagedAsync(int pageNumber, int pageSize, string? sortBy)
    {
        var (notes, totalRecords) = await _noteRepository.GetPublicNotesPagedAsync(pageNumber, pageSize, sortBy);

        return new PagedResponse<NoteDto>
        {
            Data = _mapper.Map<IEnumerable<NoteDto>>(notes),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords
        };
    }
    
    public async Task<IEnumerable<NoteDto>> GetPublicNotesByUsernameAsync(string username)
    {
        var notes = await _noteRepository.GetPublicNotesByUsernameAsync(username);
        return _mapper.Map<IEnumerable<NoteDto>>(notes);
    }
    
    public async Task SetNotePublicStatusAsync(string noteId, string userId, bool isPublic)
    {
        if (!Guid.TryParse(noteId, out var noteGuid))
        {
            throw new BadRequestException("Invalid note ID.");
        }
        
        var note = await _noteRepository.GetByIdAsync(noteGuid);
        
        if (note == null)
            throw new NotFoundException("Note not found");
        
        if (note.AuthorId != userId)
            throw new UnauthorizedException("You are not authorized to modify this note");
        
        note.IsPublic = isPublic;
        await _noteRepository.UpdateAsync(note);
    }
}