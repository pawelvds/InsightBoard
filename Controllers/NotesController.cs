using System.Security.Claims;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Services.Notes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightBoard.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var notes = await _noteService.GetAllByUserIdAsync(userId);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<NoteDto>> Create([FromBody] CreateNoteRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return Unauthorized();
        }

        var createdNote = await _noteService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetAll), new { id = createdNote.Id }, createdNote);
    }

    [HttpPatch("{id}/publish")]
    public async Task<IActionResult> Publish(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        await _noteService.PublishNoteAsync(id, userId);

        return NoContent();
    }

    [HttpPatch("{id}/unpublish")]
    public async Task<IActionResult> Unpublish(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        await _noteService.UnpublishNoteAsync(id, userId);

        return NoContent();
    }
}