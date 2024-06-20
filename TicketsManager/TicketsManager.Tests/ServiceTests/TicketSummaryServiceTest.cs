using AutoFixture;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.BLL.Mapping;
using TicketsManager.BLL.Services;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketSummaryModels;

namespace TicketsManager.Tests.ServiceTests;

[TestFixture]
public class TicketSummaryServiceTest
{
    private TicketSummaryService _ticketSummaryService;
    private Fixture _fixture;
    private IMapper _mapper;

    private Mock<ITicketSummaryRepository> _ticketSummaryRepository;
    private Mock<ITicketShareRepository> _ticketShareRepository;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TicketSummaryProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _ticketShareRepository = new Mock<ITicketShareRepository>();
        _ticketSummaryRepository = new Mock<ITicketSummaryRepository>();
        _ticketSummaryService = new TicketSummaryService(_ticketSummaryRepository.Object, _mapper, _ticketShareRepository.Object);
    }

    [Test]
    public async Task GetTicketSummariesByTicketIdAsync_WhenCalled_ReturnsTicketSummaries()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var ticketId = _fixture.Create<Guid>();
        var ticketSummaries = _fixture.CreateMany<TicketSummary>().ToList();

        var mockData = ticketSummaries.AsQueryable().BuildMock();

        _ticketSummaryRepository.Setup(x => x.GetTicketSummariesByTicketIdAsync(ticketId))
            .Returns(mockData);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketId(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _ticketSummaryService.GetTicketSummariesByTicketIdAsync(userId, ticketId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task CreateTicketSummariesAsync_WhenCalled_ReturnsTicketSummaries()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var ticketSummaries = _fixture.CreateMany<TicketSummaryCreateDto>().ToList();
        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);

        _ticketSummaryRepository.Setup(x => x.CreateTicketSummariesAsync(mappedTicketSummaries))
            .Returns(Task.CompletedTask);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketId(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _ticketSummaryService.CreateTicketSummariesAsync(userId, ticketSummaries);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateTicketSummariesAsync_WhenCalled_ReturnsTicketSummaries()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var ticketSummaries = _fixture.CreateMany<TicketSummaryFullDto>().ToList();
        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);

        _ticketSummaryRepository.Setup(x => x.UpdateTicketSummariesAsync(mappedTicketSummaries))
            .Returns(Task.CompletedTask);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketId(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _ticketSummaryService.UpdateTicketSummariesAsync(userId, ticketSummaries);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task DeleteTicketSummariesAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var ticketId = _fixture.Create<Guid>();

        _ticketSummaryRepository.Setup(x => x.DeleteTicketSummariesAsync(ticketId))
            .Returns(Task.CompletedTask);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketId(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        await _ticketSummaryService.DeleteTicketSummariesAsync(userId, ticketId);
    }
}
