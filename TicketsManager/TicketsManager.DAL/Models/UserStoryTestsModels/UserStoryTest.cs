using TicketsManager.DAL.Models.UsecaseModels;

namespace TicketsManager.DAL.Models.UserStoryTestsModels;

public class UserStoryTest
{
    public Guid Id { get; set; }
    public string TestScenarios { get; set; } = string.Empty;
    public int Order { get; set; }
    public ICollection<TestCase>? TestCases { get; set; }
    public TestPlan? TestPlan { get; set; }

    public Usecase? Usecase { get; set; }
    public Guid? UsecaseId { get; set; }
}