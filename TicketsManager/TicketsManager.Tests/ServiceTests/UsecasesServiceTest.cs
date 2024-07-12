using AutoFixture;
using AutoMapper;
using Moq;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.FullDtos;
using TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos;
using TicketsManager.BLL.Mapping;
using TicketsManager.BLL.Services;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UsecaseModels;

namespace TicketsManager.Tests.ServiceTests;

[TestFixture]
public class UsecasesServiceTest
{
    private IMapper _mapper;
    private Fixture _fixture;
    private UsecasesService _usecasesService;

    private Mock<IUsecaseRepository> _usecasesRepository;
    private Mock<ITicketShareRepository> _ticketShareRepository;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TicketUsecaseProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _ticketShareRepository = new Mock<ITicketShareRepository>();
        _usecasesRepository = new Mock<IUsecaseRepository>();
        _usecasesService = new UsecasesService(_usecasesRepository.Object, _mapper, _ticketShareRepository.Object);
    }

    [Test]
    public async Task CreateUsecasesAsync_WhenCalled_ReturnsUsecases()
    {
        // Arrange
        var createUsecasesDto = _fixture.CreateMany<CreateUsecasesDto>().ToList();
        var userIdFromToken = _fixture.Create<Guid>();

        _usecasesRepository.Setup(x => x.GetUserIdByTicketIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userIdFromToken);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _usecasesService.CreateUsecasesAsync(createUsecasesDto, userIdFromToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetUsecasesAsync_WhenCalled_ReturnsUsecases()
    {
        // Arrange
        var getUsecasesDto = _fixture.Create<GetUsecasesDto>();
        var userIdFromToken = _fixture.Create<Guid>();

        _usecasesRepository.Setup(x => x.GetUserIdByTicketIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userIdFromToken);

        _usecasesRepository.Setup(x => x.GetUsecasesAsync(It.IsAny<PaginationRequestDto>(), It.IsAny<Guid>()))
            .ReturnsAsync(_fixture.Create<PaginationResponseDto<Usecase>>());

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _usecasesService.GetUsecasesAsync(getUsecasesDto, userIdFromToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateUsecaseAsync_WhenCalled_ReturnsUsecase()
    {
        // Arrange
        var usecaseId = _fixture.Create<Guid>();
        var updateUsecaseDto = _fixture.Create<UsecaseDataFullDto>();
        var userIdFromToken = _fixture.Create<Guid>();

        var usecase = _fixture.Create<Usecase>();
        usecase.TicketId = _fixture.Create<Guid>();

        _usecasesRepository.Setup(x => x.GetUsecaseByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(usecase);

        _usecasesRepository.Setup(x => x.GetUserIdByTicketIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userIdFromToken);

        _usecasesRepository.Setup(x => x.UpdateUsecaseAsync(It.IsAny<Usecase>()))
            .Returns(Task.CompletedTask);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _usecasesService.UpdateUsecaseAsync(usecaseId, updateUsecaseDto, userIdFromToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task DeleteUsecasesByTicketIdAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var userIdFromToken = _fixture.Create<Guid>();

        _usecasesRepository.Setup(x => x.GetUserIdByTicketIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(userIdFromToken);

        _usecasesRepository.Setup(x => x.DeleteUsecasesAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        await _usecasesService.DeleteUsecasesByTicketIdAsync(ticketId, userIdFromToken);
    }
}
