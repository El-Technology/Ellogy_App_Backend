using AICommunicationService.Common.Dtos;
using AICommunicationService.Common.Helpers;
using AICommunicationService.RAG.Models;

namespace AICommunicationService.RAG.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginationResponseDto<Document>> GetDocumentsPaginatedResult(
        this IQueryable<Document> documents,
        PaginationRequestDto pagination)
    {
        return await documents
            .GetPaginatedCollection(pagination);
    }
}