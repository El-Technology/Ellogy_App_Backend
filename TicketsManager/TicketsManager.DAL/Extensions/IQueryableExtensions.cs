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
	
	private static IEnumerable<Ticket> OrderTicketsByDate(this IEnumerable<Ticket> tickets)
	{
		return tickets.OrderByDescending(e => e.UpdatedDate ?? e.CreatedDate);
	}
}
