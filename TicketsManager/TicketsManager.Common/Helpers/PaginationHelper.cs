using TicketsManager.Common.Dtos;

namespace TicketsManager.Common.Helpers;

public static class PaginationHelper
{
    public static PaginationResponseDto<T> GetPaginatedCollection<T>(this IEnumerable<T> queryable, PaginationRequestDto paginationFilter)
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
