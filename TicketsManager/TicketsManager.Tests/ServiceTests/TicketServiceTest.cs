using AutoFixture;
using AutoMapper;
using Moq;
using TicketsManager.BLL.Dtos.TicketDtos;
using TicketsManager.BLL.Exceptions;
using TicketsManager.BLL.Mapping;
using TicketsManager.BLL.Services;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.Tests.ServiceTests;

[TestFixture]
public class TicketServiceTest
{
    private IMapper _mapper;
    private Mock<ITicketsRepository> _ticketsRepository;
    private Mock<ITicketShareRepository> _ticketShareRepository;

    private Fixture _fixture;
    private TicketsService _ticketService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _ticketShareRepository = new Mock<ITicketShareRepository>();
        _ticketsRepository = new Mock<ITicketsRepository>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TicketShareProfile>();
            cfg.AddProfile<TicketProfile>();
            cfg.AddProfile<NotificationProfile>();
            cfg.AddProfile<MessageProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _ticketService = new TicketsService(_mapper, _ticketsRepository.Object, _ticketShareRepository.Object);
    }

    [Test]
    public async Task GetTicketsAsync_WhenCalled_ReturnsTickets()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var userIdFromToken = userId;
        var paginationRequestDto = _fixture.Create<PaginationRequestDto>();
        var paginationResponseDto = _fixture.Create<PaginationResponseDto<Ticket>>();

        _ticketsRepository.Setup(x => x.GetTicketsAsync(userId, paginationRequestDto))
            .ReturnsAsync(paginationResponseDto);

        // Act
        var result = await _ticketService.GetTicketsAsync(userId, paginationRequestDto, userIdFromToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void GetTicketsAsync_WhenUserIdFromTokenIsNotEqualToOwnerId_ShouldThrowException()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var userIdFromToken = _fixture.Create<Guid>();
        var paginationRequestDto = _fixture.Create<PaginationRequestDto>();

        // Act
        var ex = Assert.ThrowsAsync<ForbiddenException>(async () =>
            await _ticketService.GetTicketsAsync(userId, paginationRequestDto, userIdFromToken));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task SearchTicketsByNameAsync_WhenCalled_ReturnsTickets()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var userIdFromToken = userId;
        var searchTicketsRequestDto = _fixture.Create<SearchTicketsRequestDto>();
        var paginationResponseDto = _fixture.Create<PaginationResponseDto<Ticket>>();

        _ticketsRepository.Setup(x => x.FindTicketsAsync(userId, searchTicketsRequestDto))
            .ReturnsAsync(paginationResponseDto);

        // Act
        var result = await _ticketService.SearchTicketsByNameAsync(userId, searchTicketsRequestDto, userIdFromToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void SearchTicketsByNameAsync_WhenUserIdFromTokenIsNotEqualToOwnerId_ShouldThrowException()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var userIdFromToken = _fixture.Create<Guid>();
        var searchTicketsRequestDto = _fixture.Create<SearchTicketsRequestDto>();

        // Act
        var ex = Assert.ThrowsAsync<ForbiddenException>(async () =>
                   await _ticketService.SearchTicketsByNameAsync(userId, searchTicketsRequestDto, userIdFromToken));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task CreateTicketAsync_WhenCalled_ReturnsTicket()
    {
        // Arrange
        var createTicketRequestDto = _fixture.Create<TicketCreateRequestDto>();
        var userId = _fixture.Create<Guid>();
        var userIdFromToken = userId;
        var ticket = _fixture.Create<Ticket>();

        _ticketsRepository.Setup(x => x.CreateTicketAsync(It.IsAny<Ticket>()))
            .Callback<Ticket>(t => ticket = t);

        // Act
        var result = await _ticketService.CreateTicketAsync(createTicketRequestDto, userId, userIdFromToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void CreateTicketAsync_WhenUserIdFromTokenIsNotEqualToOwnerId_ShouldThrowException()
    {
        // Arrange
        var createTicketRequestDto = _fixture.Create<TicketCreateRequestDto>();
        var userId = _fixture.Create<Guid>();
        var userIdFromToken = _fixture.Create<Guid>();

        // Act
        var ex = Assert.ThrowsAsync<ForbiddenException>(async () =>
                   await _ticketService.CreateTicketAsync(createTicketRequestDto, userId, userIdFromToken));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void DeleteTicketAsync_WhenUserIdFromTokenIsNotEqualToOwnerId_ShouldThrowException()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var userIdFromToken = _fixture.Create<Guid>();
        var ticket = _fixture.Create<Ticket>();

        _ticketsRepository.Setup(x => x.GetTicketByIdAsync(ticketId))
            .ReturnsAsync(ticket);

        // Act
        var ex = Assert.ThrowsAsync<ForbiddenException>(async () =>
                   await _ticketService.DeleteTicketAsync(ticketId, userIdFromToken));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task DeleteTicketAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var userIdFromToken = userId;
        var ticket = _fixture.Create<Ticket>();
        ticket.UserId = userIdFromToken;

        _ticketsRepository.Setup(x => x.GetTicketByIdAsync(ticket.Id))
            .ReturnsAsync(ticket);

        // Act
        await _ticketService.DeleteTicketAsync(ticket.Id, userIdFromToken);

        // Assert
        _ticketsRepository.Verify(x => x.DeleteTicketAsync(ticket.Id), Times.Once);
    }

    [Test]
    public void DeleteTicketAsync_WhenTicketIsNull_ShouldThrowException()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var userIdFromToken = _fixture.Create<Guid>();

        _ticketsRepository.Setup(x => x.GetTicketByIdAsync(ticketId))
            .ReturnsAsync(null as Ticket);

        // Act
        var ex = Assert.ThrowsAsync<TicketNotFoundException>(async () =>
            await _ticketService.DeleteTicketAsync(ticketId, userIdFromToken));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task UpdateTicketAsync_WhenCalled_ReturnsTicket()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var ticketUpdateRequestDto = _fixture.Create<TicketUpdateRequestDto>();
        var userIdFromToken = _fixture.Create<Guid>();
        var ticket = _fixture.Create<Ticket>();
        ticket.UserId = userIdFromToken;

        _ticketsRepository.Setup(x => x.GetTicketByIdAsync(ticketId))
            .ReturnsAsync(ticket);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _ticketService.UpdateTicketAsync(ticketId, ticketUpdateRequestDto, userIdFromToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void UpdateTicketAsync_WhenTicketIsNull_ShouldThrowException()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var ticketUpdateRequestDto = _fixture.Create<TicketUpdateRequestDto>();
        var userIdFromToken = _fixture.Create<Guid>();

        _ticketsRepository.Setup(x => x.GetTicketByIdAsync(ticketId))
            .ReturnsAsync(null as Ticket);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var ex = Assert.ThrowsAsync<TicketNotFoundException>(async () =>
                   await _ticketService.UpdateTicketAsync(ticketId, ticketUpdateRequestDto, userIdFromToken));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void UpdateTicketAsync_WhenTicketUpdateIdsAreNotValid_ShouldThrowException()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var ticketUpdateRequestDto = _fixture.Create<TicketUpdateRequestDto>();
        var userIdFromToken = _fixture.Create<Guid>();
        var ticket = _fixture.Create<Ticket>();
        ticket.UserId = userIdFromToken;

        _ticketsRepository.Setup(x => x.GetTicketByIdAsync(ticketId))
            .ReturnsAsync(ticket);

        _ticketsRepository.Setup(x => x.CheckTicketUpdateIds(ticket))
            .ThrowsAsync(new Exception());

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
                          await _ticketService.UpdateTicketAsync(ticketId, ticketUpdateRequestDto, userIdFromToken));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }
}
