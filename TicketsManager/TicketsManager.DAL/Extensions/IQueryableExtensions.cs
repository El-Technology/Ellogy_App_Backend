using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginationResponseDto<Ticket>> GetFinalResultAsync(
        this IQueryable<Ticket> tickets,
        PaginationRequestDto pagination)
    {
        return await tickets
            .OrderTicketsByDate()
            .GetPaginatedCollectionAsync(pagination);
    }

    private static IQueryable<Ticket> OrderTicketsByDate(this IQueryable<Ticket> tickets)
    {
        return tickets.OrderByDescending(e => e.UpdatedDate ?? e.CreatedDate);
    }
}
