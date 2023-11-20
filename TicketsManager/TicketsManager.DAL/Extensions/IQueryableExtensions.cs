using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Extensions;

public static class QueryableExtensions
{
    public static PaginationResponseDto<Ticket> GetFinalResult(this IEnumerable<Ticket> tickets, PaginationRequestDto pagination)
    {
        return tickets
            .OrderTicketsByDate()
            .GetPaginatedCollection(pagination);
    }

    public static PaginationResponseDto<ActionHistory> GetFinalResult(this IEnumerable<ActionHistory> actionHistories, PaginationRequestDto pagination)
    {
        return actionHistories
            .OrderHistoryByDate()
            .GetPaginatedCollection(pagination);
    }

    private static IEnumerable<Ticket> OrderTicketsByDate(this IEnumerable<Ticket> tickets)
    {
        return tickets.OrderByDescending(e => e.UpdatedDate ?? e.CreatedDate);
    }

    private static IEnumerable<ActionHistory> OrderHistoryByDate(this IEnumerable<ActionHistory> actionHistories)
    {
        return actionHistories.OrderByDescending(e => e.ActionTime);
    }
}
