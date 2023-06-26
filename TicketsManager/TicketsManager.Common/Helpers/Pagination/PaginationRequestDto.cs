namespace TicketsManager.Common.Helpers.Pagination;

/// <summary>
/// Custom filter for getting fixed portions of records from database
/// </summary>
public class PaginationRequestDto
{
    /// <summary>
    /// An integer number of the current page
    /// </summary>
    public int CurrentPageNumber { get; set; } = 0;

    /// <summary>
    /// The maximum allowable size of the response data array
    /// </summary>
    public int RecordsPerPage { get; set; } = 10;
}
