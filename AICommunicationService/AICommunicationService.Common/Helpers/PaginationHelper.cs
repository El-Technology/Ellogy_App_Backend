using AICommunicationService.Common.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.Common.Helpers;

public static class PaginationHelper
{
    public static async Task<PaginationResponseDto<T>> GetPaginatedCollection<T>(this IQueryable<T> queryable,
        PaginationRequestDto paginationFilter)
        where T : class
    {
        var entities = await queryable
            .Skip((paginationFilter.CurrentPageNumber - 1) * paginationFilter.RecordsPerPage)
            .Take(paginationFilter.RecordsPerPage)
            .ToListAsync();

        return new PaginationResponseDto<T>
        {
            RecordsReturned = entities.Count,
            TotalRecordsFound = queryable.Count(),
            CurrentPageNumber = paginationFilter.CurrentPageNumber,
            RecordsPerPage = paginationFilter.RecordsPerPage,
            Data = entities
        };
    }
}