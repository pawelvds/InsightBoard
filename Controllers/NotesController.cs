using System.Security.Claims;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Exceptions;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateNoteRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        await _noteService.UpdateNoteAsync(id, request, userId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        await _noteService.DeleteNoteAsync(id, userId);
        return NoContent();
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

    [AllowAnonymous]
    [HttpGet("public/{username}")]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetPublicByUsername(string username)
    {
        try
        {
            var notes = await _noteService.GetPublicNotesByUsernameAsync(username);
            return Ok(notes);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpPatch("{id}/visibility")]
    public async Task<IActionResult> SetVisibility(string id, [FromBody] SetVisibilityRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        await _noteService.SetNotePublicStatusAsync(id, userId, request.IsPublic);
        return NoContent();
    }
}