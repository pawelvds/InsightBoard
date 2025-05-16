using AutoMapper;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Models;

namespace InsightBoard.Api.Profiles;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
        
        CreateMap<CreateNoteRequest, Note>();
    }
}