using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Models;
using InsightBoard.Api.Services.Notes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InsightBoard.Api.Controllers;

[Authorize]
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
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

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
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (userId == null)
        {
            return Unauthorized();
        }
        
        var createdNote = await _noteService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetAll), new { id = createdNote.Id }, createdNote);
    }
}