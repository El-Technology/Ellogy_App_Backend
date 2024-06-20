using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.DAL.Dtos;

public class ReturnUserStoryTestModel : UserStoryTest
{
    public string TicketSummaryData { get; set; } = string.Empty;
    public string? UsecaseTitle { get; set; }
}