namespace TicketsManager.BLL.Dtos.UserStoryTestDtos.GetDtos;

public class GetTestCaseDto
{
    public Guid Id { get; set; }
    public string TestCaseId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PreConditions { get; set; } = string.Empty;
    public string TestSteps { get; set; } = string.Empty;
    public string TestData { get; set; } = string.Empty;
    public string ExpectedResult { get; set; } = string.Empty;
}