using AutoFixture;
using AutoMapper;
using Moq;
using TicketsManager.BLL.Dtos.ActionHistoryDtos;
using TicketsManager.BLL.Mapping;
using TicketsManager.BLL.Services;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.Tests.ServiceTests;

[TestFixture]
public class ActionHistoryServiceTest
{
    private Mock<IActionHistoryRepository> _actionHistoryRepository;
    private Mock<ITicketsRepository> _ticketsRepository;

    private Fixture _fixture;
    private IMapper _mapper;
    private ActionHistoryService _actionHistoryService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _actionHistoryRepository = new Mock<IActionHistoryRepository>();
        _ticketsRepository = new Mock<ITicketsRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ActionHistoryProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _actionHistoryService = new ActionHistoryService(
            _actionHistoryRepository.Object,
            _mapper,
            _ticketsRepository.Object);
    }

    [Test]
    public async Task GetActionHistoriesAsync_WhenCalled_ReturnsActionHistories()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var searchHistoryRequestDto = _fixture.Create<SearchHistoryRequestDto>();

        _ticketsRepository.Setup(x => x.CheckIfTicketExistAsync(ticketId))
            .ReturnsAsync(true);

        _actionHistoryRepository.Setup(x => x
            .GetActionHistoriesAsync(
                ticketId,
                searchHistoryRequestDto.TicketCurrentStepEnum,
                searchHistoryRequestDto.Pagination))
            .ReturnsAsync(_fixture.Create<PaginationResponseDto<ActionHistory>>());

        // Act
        var result = await _actionHistoryService.GetActionHistoriesAsync(ticketId, searchHistoryRequestDto);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    public async Task CreateActionHistoryAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var createActionHistoryDto = _fixture.Create<CreateActionHistoryDto>();

        _ticketsRepository.Setup(x => x.CheckIfTicketExistAsync(createActionHistoryDto.TicketId))
            .ReturnsAsync(true);

        _actionHistoryRepository.Setup(x => x.CreateActionHistoryAsync(It.IsAny<ActionHistory>()))
            .Verifiable();

        // Act
        await _actionHistoryService.CreateActionHistoryAsync(createActionHistoryDto);

        // Assert
        _actionHistoryRepository.Verify(x => x.CreateActionHistoryAsync(It.IsAny<ActionHistory>()), Times.Once);
    }
}
