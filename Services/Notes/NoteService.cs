using AutoMapper;
using InsightBoard.Api.Data;
using InsightBoard.Api.DTOs.Common;
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
    
    public async Task UpdateNoteAsync(string id, UpdateNoteRequest request, string userId)
    {
        if (!Guid.TryParse(id, out var noteGuid))
        {
            throw new BadRequestException("Invalid note ID.");
        }

        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteGuid && n.AuthorId == userId);
        if (note == null)
        {
            throw new NotFoundException("Note not found.");
        }

        note.Title = request.Title;
        note.Content = request.Content;

        await _context.SaveChangesAsync();
    }
    
    public async Task DeleteNoteAsync(string id, string userId)
    {
        if (!Guid.TryParse(id, out var noteGuid))
        {
            throw new BadRequestException("Invalid note ID.");
        }

        var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == noteGuid && n.AuthorId == userId);
        if (note == null)
        {
            throw new NotFoundException("Note not found.");
        }

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
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
        var note = await _context.Notes.FindAsync(Guid.Parse(noteId));
        
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

    public async Task<PagedResponse<NoteDto>> GetPublicNotesPagedAsync(int pageNumber, int pageSize, string? sortBy)
    {
        var query = _context.Notes
            .Where(n => n.IsPublic);

        query = sortBy switch
        {
            "created_at" => query.OrderByDescending(n => n.CreatedAt),
            "created_at_desc" => query.OrderByDescending(n => n.CreatedAt),
            _ => query.OrderByDescending(n => n.CreatedAt)
        };
        
        var totalrecords = await query.CountAsync();
        
        var notes = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<NoteDto>
        {
            Data = _mapper.Map<IEnumerable<NoteDto>>(notes),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalrecords
        };
    }
    
    public async Task<IEnumerable<NoteDto>> GetPublicNotesByUsernameAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
            throw new NotFoundException("User not found.");

        return await _context.Notes
            .Where(n => n.AuthorId == user.Id && n.IsPublic)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoteDto
            {
                Id = $"{n.Id}",
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                IsPublic = n.IsPublic
            })
            .ToListAsync();
    }
    
    public async Task SetNotePublicStatusAsync(string noteId, string userId, bool isPublic)
    {
        var note = await _context.Notes
            .FirstOrDefaultAsync(n => n.Id.ToString() == noteId && n.AuthorId == userId);

        if (note == null)
            throw new NotFoundException("Note not found or access denied.");

        note.IsPublic = isPublic;
        await _context.SaveChangesAsync();
    }
}