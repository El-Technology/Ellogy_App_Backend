namespace TicketsManager.BLL.Dtos.UserStoryTestDtos;

public class CreateUserStoryTestDto
{
    public Guid TicketSummaryId { get; set; }

    public string TestScenarios { get; set; } = string.Empty;
    public IEnumerable<CreateTestCaseDto>? TestCases { get; set; }
    public CreateTestPlanDto? TestPlan { get; set; }
    public Guid? UsecaseId { get; set; }
}