using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.DAL.Dtos;

public class ReturnUserStoryTestModel : UserStoryTest
{
    public string TicketSummaryData { get; set; } = string.Empty;
    public string? UsecaseTitle { get; set; }
}