namespace TicketsManager.Common.Dtos;

public class SearchTicketsRequestDto
{
    public string TicketTitle { get; set; }

    public PaginationRequestDto Pagination { get; set; }
}
