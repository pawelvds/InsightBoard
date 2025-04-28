using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Services.Notes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightBoard.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class PublicNoteController : ControllerBase
{
    private readonly INoteService _noteService;

    public PublicNoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NoteDto>>> GetPublicNotes()
    {
        var notes = await _noteService.GetPublicNotesAsync();
        return Ok(notes);
    }
}