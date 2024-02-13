namespace AICommunicationService.Common.Dtos;

/// <summary>
///     Custom class that represents the paginated response
/// </summary>
/// <typeparam name="T"></typeparam>
public class PaginationResponseDto<T>
{
    /// <summary>
    ///     The number of records returned in a particular case
    /// </summary>
    public int RecordsReturned { get; set; }

    /// <summary>
    ///     Total count of records in the corresponding table
    /// </summary>
    public int TotalRecordsFound { get; set; }

    /// <summary>
    ///     An integer number of the current page
    /// </summary>
    public int CurrentPageNumber { get; set; }

    /// <summary>
    ///     The maximum allowable size of the response data array
    /// </summary>
    public int RecordsPerPage { get; set; }

    /// <summary>
    ///     An expectable collection of records in response
    /// </summary>
    public List<T> Data { get; set; }
}