using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Models;
using InsightBoard.Api.Services.Notes;
using Microsoft.AspNetCore.Mvc;

namespace InsightBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;

    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
    {
        var notes = await _noteService.GetAllAsync();
        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteRequest request)
    {
        var createdNote = await _noteService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = createdNote.Id }, createdNote);
    }
}