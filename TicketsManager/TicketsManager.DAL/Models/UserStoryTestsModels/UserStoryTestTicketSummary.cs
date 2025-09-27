using TicketsManager.DAL.Models.TicketSummaryModels;

namespace TicketsManager.DAL.Models.UserStoryTestsModels;

/// <summary>
/// Junction table linking UserStoryTests to TicketSummaries for explicit traceability relationships.
/// </summary>
public class UserStoryTestTicketSummary
{
    public Guid Id { get; set; }
    public Guid UserStoryTestId { get; set; }
    public Guid TicketSummaryId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public UserStoryTest UserStoryTest { get; set; } = null!;
    public TicketSummary TicketSummary { get; set; } = null!;
}