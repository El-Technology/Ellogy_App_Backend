namespace TicketsManager.Common.Dtos;

/// <summary>
/// Custom filter for getting fixed portions of records from database
/// </summary>
public class PaginationRequestDto
{
    private int _currentPageNumber = 1;
    private int _recordsPerPage = 10;

    private const int MinimumValue = 1;

    /// <summary>
    /// An integer number of the current page
    /// </summary>
    public int CurrentPageNumber
    {
        get => _currentPageNumber;
        set => _currentPageNumber = value < MinimumValue ? _currentPageNumber : value;
    }

    /// <summary>
    /// The maximum allowable size of the response data array
    /// </summary>
    public int RecordsPerPage
    {
        get => _recordsPerPage;
        set => _recordsPerPage = value < MinimumValue ? _recordsPerPage : value;
    }
}
