namespace PaymentManager.Common.Dtos;

public class StripePaginationRequestDto
{
    public int RecordsPerPage { get; set; } = 10;

    public string? StartAfter { get; set; } = null;
    public string? EndBefore { get; set; } = null;
}