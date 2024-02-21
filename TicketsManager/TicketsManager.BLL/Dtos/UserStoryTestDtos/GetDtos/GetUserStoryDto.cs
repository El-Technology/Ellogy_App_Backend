namespace TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;

public class GetUserStoryDto
{
    public Guid Id { get; set; }
    public string TestScenarios { get; set; } = string.Empty;
    public IEnumerable<GetTestCaseDto>? TestCases { get; set; }
    public GetTestPlanDto? TestPlan { get; set; }
    public string TicketSummaryData { get; set; } = string.Empty;
    public Guid TicketSummaryId { get; set; }
}