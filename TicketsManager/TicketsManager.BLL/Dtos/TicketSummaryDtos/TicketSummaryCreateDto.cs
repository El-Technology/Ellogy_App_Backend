using TicketsManager.BLL.Dtos.SummaryAcceptanceCriteriaDtos;
using TicketsManager.BLL.Dtos.SummaryScenarioDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketSummaryDtos;

public class TicketSummaryCreateDto
{
    /// <summary>
    ///     The id of the ticket summary.
    /// </summary>
    public Guid TicketId { get; set; }

    /// <summary>
    ///     The data of the ticket summary.
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    ///     Indicates if the ticket summary is a potential.
    /// </summary>
    public bool IsPotential { get; set; }

    /// <summary>
    ///     Shows at which sub - stage the message was sent
    /// </summary>
    public SubStageEnum? SubStage { get; set; }

    /// <summary>
    ///     The scenarios associated with the ticket summary.
    /// </summary>
    public IEnumerable<SummaryScenarioCreateDto>? SummaryScenarios { get; set; }

    /// <summary>
    ///     The acceptance criteria associated with the ticket summary.
    /// </summary>
    public IEnumerable<SummaryAcceptanceCriteriaCreateDto>? SummaryAcceptanceCriteria { get; set; }
}