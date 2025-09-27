namespace TicketsManager.BLL.Dtos.TraceabilityDtos;

public class TraceabilityStoryDto
{
    public string UserStoryId { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string SubStage { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IEnumerable<TraceabilityScenarioDto> Scenarios { get; set; } = new List<TraceabilityScenarioDto>();
    public IEnumerable<TraceabilityAcceptanceDto> AcceptanceCriteria { get; set; } = new List<TraceabilityAcceptanceDto>();
    public IEnumerable<TraceabilityTestDto> Tests { get; set; } = new List<TraceabilityTestDto>();
    public TraceabilityCoverageDto Coverage { get; set; } = new();
}

public class TraceabilityScenarioDto
{
    public string ScenarioId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TraceabilityAcceptanceDto
{
    public string AcceptanceId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class TraceabilityTestDto
{
    public string UserStoryTestId { get; set; } = string.Empty;
    public string UsecaseId { get; set; } = string.Empty;
    public string UsecaseTitle { get; set; } = string.Empty;
    public TraceabilityTestPlanDto? TestPlan { get; set; }
    public IEnumerable<string> TestScenarios { get; set; } = new List<string>();
    public IEnumerable<TraceabilityTestCaseDto> TestCases { get; set; } = new List<TraceabilityTestCaseDto>();
}

public class TraceabilityTestPlanDto
{
    public string Id { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string Resources { get; set; } = string.Empty;
    public string Schedule { get; set; } = string.Empty;
    public string TestEnvironment { get; set; } = string.Empty;
    public string RiskManagement { get; set; } = string.Empty;
}

public class TraceabilityTestCaseDto
{
    public string TestCaseId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PreConditions { get; set; } = string.Empty;
    public IEnumerable<string> TestSteps { get; set; } = new List<string>();
    public string TestData { get; set; } = string.Empty;
    public string ExpectedResult { get; set; } = string.Empty;
}

public class TraceabilityCoverageDto
{
    public string Status { get; set; } = "covered"; // "covered", "missing-tests", "missing-criteria"
    public int ScenarioCount { get; set; }
    public int AcceptanceCriteriaCount { get; set; }
    public int TestCaseCount { get; set; }
}