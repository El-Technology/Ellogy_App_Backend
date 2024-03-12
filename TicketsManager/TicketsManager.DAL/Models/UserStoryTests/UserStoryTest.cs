namespace TicketsManager.DAL.Models.UserStoryTests;

public class UserStoryTest
{
    public Guid Id { get; set; }
    public string TestScenarios { get; set; } = string.Empty;
    public ICollection<TestCase>? TestCases { get; set; }
    public TestPlan? TestPlan { get; set; }

    public Usecase? Usecase { get; set; }
    public Guid? UsecaseId { get; set; }
}