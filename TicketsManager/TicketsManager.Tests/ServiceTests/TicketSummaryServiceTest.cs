using AutoFixture;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.BLL.Mapping;
using TicketsManager.BLL.Services;
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

        _ticketSummaryRepository = new Mock<ITicketSummaryRepository>();
        _ticketSummaryService = new TicketSummaryService(_ticketSummaryRepository.Object, _mapper);
    }

    [Test]
    public async Task GetTicketSummariesByTicketIdAsync_WhenCalled_ReturnsTicketSummaries()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var ticketSummaries = _fixture.CreateMany<TicketSummary>().ToList();

        var mockData = ticketSummaries.AsQueryable().BuildMock();

        _ticketSummaryRepository.Setup(x => x.GetTicketSummariesByTicketIdAsync(ticketId))
            .Returns(mockData);

        // Act
        var result = await _ticketSummaryService.GetTicketSummariesByTicketIdAsync(ticketId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task CreateTicketSummariesAsync_WhenCalled_ReturnsTicketSummaries()
    {
        // Arrange
        var ticketSummaries = _fixture.CreateMany<TicketSummaryCreateDto>().ToList();
        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);

        _ticketSummaryRepository.Setup(x => x.CreateTicketSummariesAsync(mappedTicketSummaries))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _ticketSummaryService.CreateTicketSummariesAsync(ticketSummaries);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateTicketSummariesAsync_WhenCalled_ReturnsTicketSummaries()
    {
        // Arrange
        var ticketSummaries = _fixture.CreateMany<TicketSummaryFullDto>().ToList();
        var mappedTicketSummaries = _mapper.Map<List<TicketSummary>>(ticketSummaries);

        _ticketSummaryRepository.Setup(x => x.UpdateTicketSummariesAsync(mappedTicketSummaries))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _ticketSummaryService.UpdateTicketSummariesAsync(ticketSummaries);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task DeleteTicketSummariesAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();

        _ticketSummaryRepository.Setup(x => x.DeleteTicketSummariesAsync(ticketId))
            .Returns(Task.CompletedTask);

        // Act
        await _ticketSummaryService.DeleteTicketSummariesAsync(ticketId);
    }
}
