using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Models.TicketModels;
using TicketsManager.DAL.Models.UsecaseModels;

namespace TicketsManager.DAL.Models.TicketSummaryModels;

public class TicketSummary
{
    public Guid Id { get; set; }
    public string Data { get; set; }
    public bool IsPotential { get; set; }
    public SubStageEnum? SubStage { get; set; }
    public ICollection<SummaryScenario> SummaryScenarios { get; set; } = new List<SummaryScenario>();
    public ICollection<SummaryAcceptanceCriteria> SummaryAcceptanceCriteria { get; set; } = new List<SummaryAcceptanceCriteria>();
    public ICollection<Usecase> Usecases { get; set; } = new List<Usecase>();

    public Guid TicketId { get; set; }
    public Ticket Ticket { get; set; }
}