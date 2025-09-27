using AutoMapper;
using TicketsManager.BLL.Dtos.TraceabilityDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.BLL.Services;

public class TraceabilityService : ITraceabilityService
{
    private readonly IMapper _mapper;
    private readonly ITraceabilityRepository _traceabilityRepository;
    private readonly ITicketShareRepository _ticketShareRepository;
    private readonly ITicketsRepository _ticketsRepository;

    public TraceabilityService(
        IMapper mapper,
        ITraceabilityRepository traceabilityRepository,
        ITicketShareRepository ticketShareRepository,
        ITicketsRepository ticketsRepository)
    {
        _mapper = mapper;
        _traceabilityRepository = traceabilityRepository;
        _ticketShareRepository = ticketShareRepository;
        _ticketsRepository = ticketsRepository;
    }

    public async Task<TraceabilityMatrixResponseDto> GetTraceabilityMatrixAsync(Guid ticketId, Guid userIdFromToken)
    {
        // Validate user has access to the ticket
        await ValidateUserPermissionAsync(ticketId, userIdFromToken);

        // Verify ticket exists
        var ticket = await _ticketsRepository.GetTicketByIdAsync(ticketId);
        if (ticket == null)
            throw new NotFoundException($"Ticket with ID {ticketId} not found");

        // Get raw traceability data from the database view
        var rawData = await _traceabilityRepository.GetTraceabilityMatrixViewAsync(ticketId);

        // Transform the raw data into grouped story DTOs
        var stories = TransformRawDataToStories(rawData);

        return new TraceabilityMatrixResponseDto
        {
            Stories = stories
        };
    }

    #region Private Methods

    private async Task ValidateUserPermissionAsync(Guid ticketId, Guid userIdFromToken)
    {
        try
        {
            await _ticketShareRepository.CheckIfUserHaveAccessToComponentAsync(
                ticketId,
                userIdFromToken,
                TicketCurrentStepEnum.TestCases, // User needs at least test cases access to view traceability
                SharePermissionEnum.Read);
        }
        catch
        {
            throw new ForbiddenException(userIdFromToken);
        }
    }

    private static IEnumerable<TraceabilityStoryDto> TransformRawDataToStories(IEnumerable<TraceabilityMatrixView> rawData)
    {
        var groupedByStory = rawData
            .GroupBy(x => x.UserStoryId)
            .Select(storyGroup =>
            {
                var storyData = storyGroup.First();

                // Group scenarios by ScenarioId
                var scenarios = storyGroup
                    .Where(x => x.ScenarioId.HasValue)
                    .GroupBy(x => x.ScenarioId)
                    .Select(scenarioGroup =>
                    {
                        var scenario = scenarioGroup.First();
                        return new TraceabilityScenarioDto
                        {
                            ScenarioId = scenario.ScenarioId.ToString() ?? string.Empty,
                            Title = scenario.ScenarioTitle ?? string.Empty,
                            Description = scenario.ScenarioDescription ?? string.Empty
                        };
                    })
                    .Distinct()
                    .ToList();

                // Group acceptance criteria by AcceptanceId
                var acceptanceCriteria = storyGroup
                    .Where(x => x.AcceptanceId.HasValue)
                    .GroupBy(x => x.AcceptanceId)
                    .Select(acceptanceGroup =>
                    {
                        var acceptance = acceptanceGroup.First();
                        return new TraceabilityAcceptanceDto
                        {
                            AcceptanceId = acceptance.AcceptanceId.ToString() ?? string.Empty,
                            Title = acceptance.AcceptanceTitle ?? string.Empty,
                            Description = acceptance.AcceptanceDescription ?? string.Empty
                        };
                    })
                    .Distinct()
                    .ToList();

                // Group tests by UserStoryTestId
                var tests = storyGroup
                    .Where(x => x.UserStoryTestId.HasValue)
                    .GroupBy(x => x.UserStoryTestId)
                    .Select(testGroup =>
                    {
                        var testData = testGroup.First();

                        // Group test cases by TestCaseId within this test
                        var testCases = testGroup
                            .Where(x => x.TestCaseId.HasValue)
                            .Select(tc => new TraceabilityTestCaseDto
                            {
                                TestCaseId = tc.TestCaseRef ?? string.Empty,
                                Description = tc.TestCaseDescription ?? string.Empty,
                                PreConditions = tc.PreConditions ?? string.Empty,
                                TestSteps = (tc.TestSteps ?? string.Empty).Split('\n', StringSplitOptions.RemoveEmptyEntries),
                                TestData = tc.TestData ?? string.Empty,
                                ExpectedResult = tc.ExpectedResult ?? string.Empty
                            })
                            .Distinct()
                            .ToList();

                        return new TraceabilityTestDto
                        {
                            UserStoryTestId = testData.UserStoryTestId.ToString() ?? string.Empty,
                            UsecaseId = testData.UsecaseId.ToString() ?? string.Empty,
                            UsecaseTitle = "Generated Test Plan", // This would need to be joined from Usecase table
                            TestPlan = null, // Would need to be joined from TestPlan table
                            TestScenarios = new List<string>(), // Would need to be parsed from TestScenarios field
                            TestCases = testCases
                        };
                    })
                    .Distinct()
                    .ToList();

                // Calculate coverage
                var coverage = new TraceabilityCoverageDto
                {
                    ScenarioCount = scenarios.Count,
                    AcceptanceCriteriaCount = acceptanceCriteria.Count,
                    TestCaseCount = tests.SelectMany(t => t.TestCases).Count(),
                    Status = CalculateCoverageStatus(scenarios.Count, acceptanceCriteria.Count, tests.SelectMany(t => t.TestCases).Count())
                };

                return new TraceabilityStoryDto
                {
                    UserStoryId = storyData.UserStoryId.ToString(),
                    TicketId = storyData.TicketId.ToString(),
                    Title = storyData.UserStory,
                    SubStage = storyData.SubStage ?? string.Empty,
                    Description = storyData.UserStory, // Using same as title for now
                    Scenarios = scenarios,
                    AcceptanceCriteria = acceptanceCriteria,
                    Tests = tests,
                    Coverage = coverage
                };
            })
            .ToList();

        return groupedByStory;
    }

    private static string CalculateCoverageStatus(int scenarioCount, int acceptanceCriteriaCount, int testCaseCount)
    {
        if (testCaseCount == 0)
            return "missing-tests";

        if (acceptanceCriteriaCount == 0)
            return "missing-criteria";

        return "covered";
    }

    #endregion
}