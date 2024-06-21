using AutoFixture;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Mapping;
using TicketsManager.BLL.Services;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UserStoryTestsModels;

namespace TicketsManager.Tests.ServiceTests;

[TestFixture]
public class UserStoryTestServiceTest
{
    private Fixture _fixture;
    private IMapper _mapper;
    private UserStoryTestService _userStoryTestService;

    private Mock<ITicketShareRepository> _ticketShareRepository;
    private Mock<IUserStoryTestRepository> _userStoryTestRepository;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserStoryTestProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _ticketShareRepository = new Mock<ITicketShareRepository>();
        _userStoryTestRepository = new Mock<IUserStoryTestRepository>();
        _userStoryTestService = new UserStoryTestService(_userStoryTestRepository.Object, _mapper, _ticketShareRepository.Object);
    }

    [Test]
    public async Task GetUserStoryTestsAsync_WhenCalled_ReturnsUserStoryTests()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();
        var userId = _fixture.Create<Guid>();
        var userStoryTestsMock = _fixture.CreateMany<ReturnUserStoryTestModel>().BuildMock();

        _userStoryTestRepository.Setup(x => x.GetUserStoryTests(ticketId))
            .Returns(userStoryTestsMock);
        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userStoryTestService.GetUserStoryTestsAsync(userId, ticketId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateUserStoryTestAsync_WhenCalled_ReturnsUserStoryTest()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var updateUserStoryTestDtos = _fixture.Create<List<UpdateUserStoryTestDto>>();

        _userStoryTestRepository.Setup(x => x.UpdateUserStoryTestAsync(It.IsAny<List<UserStoryTest>>()))
            .Returns(Task.CompletedTask);
        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketIdAsync(
           It.IsAny<Guid>(),
           It.IsAny<Guid>(),
           It.IsAny<TicketCurrentStepEnum>(),
           It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        await _userStoryTestService.UpdateUserStoryTestAsync(userId, updateUserStoryTestDtos);

        // Assert
        _userStoryTestRepository.Verify(x => x.UpdateUserStoryTestAsync(It.IsAny<List<UserStoryTest>>()), Times.Once);
    }

    [Test]
    public async Task DeleteUserStoryTestAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var ticketId = _fixture.Create<Guid>();

        _userStoryTestRepository.Setup(x => x.DeleteUserStoryTestByTicketIdAsync(ticketId))
            .Returns(Task.CompletedTask);
        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketIdAsync(
           It.IsAny<Guid>(),
           It.IsAny<Guid>(),
           It.IsAny<TicketCurrentStepEnum>(),
           It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        await _userStoryTestService.DeleteUserStoryTestAsync(userId, ticketId);

        // Assert
        _userStoryTestRepository.Verify(x => x.DeleteUserStoryTestByTicketIdAsync(ticketId), Times.Once);
    }

    [Test]
    public async Task DeleteTestCasesByIdsAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var testCasesIds = _fixture.Create<List<Guid>>();

        _userStoryTestRepository.Setup(x => x.DeleteTestCasesByIds(testCasesIds))
            .Returns(Task.CompletedTask);
        _ticketShareRepository.Setup(x => x.CheckIfUserHaveAccessToComponentByTicketIdAsync(
            It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<TicketCurrentStepEnum>(),
            It.IsAny<SharePermissionEnum>())).Returns(Task.CompletedTask);

        // Act
        await _userStoryTestService.DeleteTestCasesByIdsAsync(userId, testCasesIds);

        // Assert
        _userStoryTestRepository.Verify(x => x.DeleteTestCasesByIds(testCasesIds), Times.Once);
    }
}
