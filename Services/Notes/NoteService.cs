using AutoMapper;
using InsightBoard.Api.Data;
using InsightBoard.Api.DTOs.Notes;
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

    public async Task<IEnumerable<NoteDto>> GetAllAsync()
    {
        var notes = await _context.Notes
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        
        return _mapper.Map<IEnumerable<NoteDto>>(notes);
    }
    
    public async Task<NoteDto> CreateAsync(CreateNoteRequest request)
    {
        var note = new Note
        {
            Title = request.Title,
            Content = request.Content,
        };
        
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<NoteDto>(note);
    }
}