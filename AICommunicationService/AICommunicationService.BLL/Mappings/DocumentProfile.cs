using AICommunicationService.BLL.Dtos;
using AICommunicationService.RAG.Models;
using AutoMapper;

namespace AICommunicationService.BLL.Mappings
{
    public class DocumentProfile : Profile
    {
        public DocumentProfile()
        {
            CreateMap<Document, DocumentResponseDto>();
        }
    }
}
