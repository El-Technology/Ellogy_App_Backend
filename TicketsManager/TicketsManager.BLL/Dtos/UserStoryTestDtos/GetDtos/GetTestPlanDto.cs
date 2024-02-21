namespace TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;

public class GetTestPlanDto
{
    public Guid Id { get; set; }
    public string Objective { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string Resources { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public string TestEnvironment { get; set; } = string.Empty;
    public string RiskManagement { get; set; } = string.Empty;
}