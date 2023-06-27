using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;

namespace TicketsManager.Common.Helpers.Pagination;

public static class PaginationHelper
{
    public static async Task<PaginationResponseDto<T>> GetPaginatedQueryableAsync<T>(this IQueryable<T> queryable, PaginationRequestDto paginationFilter)
        where T : class
    {
        var entities = await queryable
            .Skip((paginationFilter.CurrentPageNumber - 1) * paginationFilter.RecordsPerPage)
            .Take(paginationFilter.RecordsPerPage)
            .ToListAsync();

        return new()
        {
            RecordsReturned = entities.Count,
            TotalRecordsFound = await queryable.CountAsync(),
            CurrentPageNumber = paginationFilter.CurrentPageNumber,
            RecordsPerPage = paginationFilter.RecordsPerPage,
            Data = entities
        };
    }

    public static PaginationResponseDto<T> GetPaginatedCollectionAsync<T>(this IEnumerable<T> queryable, PaginationRequestDto paginationFilter)
        where T : class
    {
        var entities = queryable
            .Skip((paginationFilter.CurrentPageNumber - 1) * paginationFilter.RecordsPerPage)
            .Take(paginationFilter.RecordsPerPage)
            .ToList();

        return new()
        {
            RecordsReturned = entities.Count,
            TotalRecordsFound = queryable.Count(),
            CurrentPageNumber = paginationFilter.CurrentPageNumber,
            RecordsPerPage = paginationFilter.RecordsPerPage,
            Data = entities
        };
    }
}
