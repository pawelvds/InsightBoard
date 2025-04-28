using AutoMapper;
using InsightBoard.Api.Data;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Exceptions;
using InsightBoard.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InsightBoard.Api.Services.Notes;

public class NoteService: INoteService
{
    private readonly InsightBoardDbContext _context;
    private readonly IMapper _mapper;

    public NoteService(InsightBoardDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<IEnumerable<NoteDto>> GetAllByUserIdAsync(string userId)
    {
        var notes = await _context.Notes
            .Where(n => n.AuthorId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

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
        
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<NoteDto>(note);
    }

    public async Task<IEnumerable<NoteDto>> GetPublicNotesAsync()
    {
        var publicNotes = await _context.Notes
            .Where(n => n.IsPublic)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<NoteDto>>(publicNotes);
    }

    public async Task PublishNoteAsync(string noteId, string userId)
    {
        var note = await _context.Notes.FindAsync(noteId);
        
        if (note == null)
            throw new NotFoundException("Note not found");
        
        if (note.AuthorId != userId)
            throw new UnauthorizedException("You are not authorized to view this note");
        
        note.IsPublic = true;
        await _context.SaveChangesAsync();
    }

    public async Task UnpublishNoteAsync(string noteId, string userId)
    {
        var note = await _context.Notes.FindAsync(noteId);
        
        if (note == null)
            throw new NotFoundException("Note not found");
        
        if (note.AuthorId != userId)
            throw new UnauthorizedException("You are not authorized to view this note");
        
        note.IsPublic = false;
        await _context.SaveChangesAsync();
    }
}