namespace TicketsManager.DAL.Models.TicketSummaryModels;

public class SummaryScenario
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Guid TicketSummaryId { get; set; }
    public TicketSummary? TicketSummary { get; set; }
}