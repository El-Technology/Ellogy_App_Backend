namespace AICommunicationService.Common.Dtos;

public class PaginationRequestDto
{
    private const int MINIMUM_VALUE = 1;
    private int _currentPageNumber = 1;
    private int _recordsPerPage = 10;

    /// <summary>
    ///     An integer number of the current page
    /// </summary>
    public int CurrentPageNumber
    {
        get => _currentPageNumber;
        set => _currentPageNumber = value < MINIMUM_VALUE ? _currentPageNumber : value;
    }

    /// <summary>
    ///     The maximum allowable size of the response data array
    /// </summary>
    public int RecordsPerPage
    {
        get => _recordsPerPage;
        set => _recordsPerPage = value < MINIMUM_VALUE ? _recordsPerPage : value;
    }
}