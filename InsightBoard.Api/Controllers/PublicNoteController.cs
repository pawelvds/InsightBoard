using InsightBoard.Api.DTOs.Common;
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
    public async Task<ActionResult<PagedResponse<NoteDto>>> GetPublicNotes(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = "createdAt_desc")
    {
        var notes = await _noteService.GetPublicNotesPagedAsync(pageNumber, pageSize, sortBy);
        return Ok(notes);
    }

}