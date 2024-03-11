using TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;

namespace TicketsManager.BLL.Dtos.UserStoryTestDtos;
public class UpdateUserStoryTestDto
{
    public Guid Id { get; set; }
    public string TestScenarios { get; set; } = string.Empty;
    public IEnumerable<GetTestCaseDto>? TestCases { get; set; }
    public GetTestPlanDto? TestPlan { get; set; }
    public Guid TicketSummaryId { get; set; }
    public Guid? UsecaseId { get; set; }
}
