using TicketsManager.DAL.Enums;

namespace TicketsManager.DAL.Models;

public class TicketSummary
{
    public Guid Id { get; set; }
    public string Data { get; set; }
    public bool IsPotential { get; set; }
    public SubStageEnum? SubStage { get; set; }
    public IEnumerable<SummaryScenario>? SummaryScenarios { get; set; }
    public IEnumerable<SummaryAcceptanceCriteria>? SummaryAcceptanceCriteria { get; set; }

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; }
}