using AICommunicationService.BLL.Dtos;
using AICommunicationService.RAG.Models;
using AutoMapper;

namespace AICommunicationService.BLL.Mappings;

public class DocumentProfile : Profile
{
    public DocumentProfile()
    {
        CreateMap<Document, DocumentResponseDto>();

        CreateMap<Document, DocumentResponseWithOwner>()
            .ForMember(dest => dest.Email, opt =>
                opt.Ignore())
            .ForMember(dest => dest.FirstName, opt =>
                opt.Ignore())
            .ForMember(dest => dest.LastName, opt =>
                opt.Ignore())
            .ForMember(dest => dest.AvatarLink, opt =>
                opt.Ignore());
    }
}