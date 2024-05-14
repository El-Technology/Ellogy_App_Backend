using AICommunicationService.Common.Dtos;
using AICommunicationService.Common.Helpers;
using AICommunicationService.DAL.Models;

namespace AICommunicationService.DAL.Extensions;

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