using AutoFixture;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using TicketsManager.BLL.Dtos.UserStoryTestDtos;
using TicketsManager.BLL.Mapping;
using TicketsManager.BLL.Services;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.UserStoryTests;

namespace TicketsManager.Tests.ServiceTests;

[TestFixture]
public class UserStoryTestServiceTest
{
    private Fixture _fixture;
    private IMapper _mapper;
    private UserStoryTestService _userStoryTestService;

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

        _userStoryTestRepository = new Mock<IUserStoryTestRepository>();
        _userStoryTestService = new UserStoryTestService(_userStoryTestRepository.Object, _mapper);
    }

    //[Test]
    //public async Task AddUserStoryTestAsync_WhenCalled_ReturnsUserStoryTest()
    //{
    //    // Arrange
    //    var createUserStoryTestDto = _fixture.Create<List<CreateUserStoryTestDto>>();
    //    var userStoryTestReturnsEmptyDto = Enumerable.Empty<ReturnUserStoryTestModel>().BuildMock();

    //    _userStoryTestRepository.Setup(x => x.GetUserStoryTests(It.IsAny<List<UserStoryTest>>()))
    //        .Returns(userStoryTestReturnsEmptyDto);

    //    _userStoryTestRepository.Setup(x => x.AddUserStoryTestAsync(It.IsAny<List<UserStoryTest>>()))
    //        .Returns(Task.CompletedTask);

    //    // Act
    //    var result = await _userStoryTestService.AddUserStoryTestAsync(createUserStoryTestDto);

    //    // Assert
    //    _userStoryTestRepository.Verify(x => x.GetUserStoryTests(It.IsAny<List<UserStoryTest>>()), Times.AtMost(2));
    //    _userStoryTestRepository.Verify(x => x.AddUserStoryTestAsync(It.IsAny<List<UserStoryTest>>()), Times.Once);

    //    Assert.That(result, Is.Not.Null);
    //}

    [Test]
    public async Task GetUserStoryTestsAsync_WhenCalled_ReturnsUserStoryTests()
    {
        // Arrange
        var userStoryId = _fixture.Create<Guid>();
        var userStoryTestsMock = _fixture.CreateMany<ReturnUserStoryTestModel>().BuildMock();

        _userStoryTestRepository.Setup(x => x.GetUserStoryTests(userStoryId))
            .Returns(userStoryTestsMock);

        // Act
        var result = await _userStoryTestService.GetUserStoryTestsAsync(userStoryId);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task UpdateUserStoryTestAsync_WhenCalled_ReturnsUserStoryTest()
    {
        // Arrange
        var updateUserStoryTestDtos = _fixture.Create<List<UpdateUserStoryTestDto>>();

        _userStoryTestRepository.Setup(x => x.UpdateUserStoryTestAsync(It.IsAny<List<UserStoryTest>>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userStoryTestService.UpdateUserStoryTestAsync(updateUserStoryTestDtos);

        // Assert
        _userStoryTestRepository.Verify(x => x.UpdateUserStoryTestAsync(It.IsAny<List<UserStoryTest>>()), Times.Once);
    }

    [Test]
    public async Task DeleteUserStoryTestAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var ticketId = _fixture.Create<Guid>();

        _userStoryTestRepository.Setup(x => x.DeleteUserStoryTestByTicketIdAsync(ticketId))
            .Returns(Task.CompletedTask);

        // Act
        await _userStoryTestService.DeleteUserStoryTestAsync(ticketId);

        // Assert
        _userStoryTestRepository.Verify(x => x.DeleteUserStoryTestByTicketIdAsync(ticketId), Times.Once);
    }

    [Test]
    public async Task DeleteTestCasesByIdsAsync_WhenCalled_ReturnsNothing()
    {
        // Arrange
        var testCasesIds = _fixture.Create<List<Guid>>();

        _userStoryTestRepository.Setup(x => x.DeleteTestCasesByIds(testCasesIds))
            .Returns(Task.CompletedTask);

        // Act
        await _userStoryTestService.DeleteTestCasesByIdsAsync(testCasesIds);

        // Assert
        _userStoryTestRepository.Verify(x => x.DeleteTestCasesByIds(testCasesIds), Times.Once);
    }
}
