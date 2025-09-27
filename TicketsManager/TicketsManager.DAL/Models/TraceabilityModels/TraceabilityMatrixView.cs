using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketsManager.DAL.Models.TicketModels;

/// <summary>
/// Represents a row from the vw_TraceabilityMatrix database view.
/// This view aggregates user stories, scenarios, acceptance criteria, and linked test cases.
/// </summary>
[Keyless]
[Table("vw_TraceabilityMatrix")]
public class TraceabilityMatrixView
{
    public Guid TicketId { get; set; }

    public Guid UserStoryId { get; set; }
    public string UserStory { get; set; } = string.Empty;
    public int? SubStage { get; set; }

    public Guid? ScenarioId { get; set; }
    public string? ScenarioTitle { get; set; }
    public string? ScenarioDescription { get; set; }

    public Guid? AcceptanceId { get; set; }
    public string? AcceptanceTitle { get; set; }
    public string? AcceptanceDescription { get; set; }

    public Guid? UserStoryTestId { get; set; }
    public int? TestOrder { get; set; }
    public Guid? UsecaseId { get; set; }

    public Guid? TestCaseId { get; set; }
    public string? TestCaseRef { get; set; }
    public string? TestCaseDescription { get; set; }
    public string? PreConditions { get; set; }
    public string? TestSteps { get; set; }
    public string? TestData { get; set; }
    public string? ExpectedResult { get; set; }
}
