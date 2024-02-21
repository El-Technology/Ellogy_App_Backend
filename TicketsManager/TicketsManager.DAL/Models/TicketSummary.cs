using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.DAL.Models;

public class TicketSummary
{
    public Guid Id { get; set; }
    public string Data { get; set; }
    public bool IsPotential { get; set; }
    public SubStageEnum? SubStage { get; set; }
    public ICollection<SummaryScenario>? SummaryScenarios { get; set; }
    public ICollection<SummaryAcceptanceCriteria>? SummaryAcceptanceCriteria { get; set; }
    public UserStoryTest? UserStoryTest { get; set; }

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; }
}