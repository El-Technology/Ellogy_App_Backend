using AutoFixture;
using Moq;
using UserManager.BLL.Services;
using UserManager.Common.Dtos;
using UserManager.DAL.Enums;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.Tests.ServiceTests;
[TestFixture]
public class UserExternalServiceTests
{
    private Mock<IUserExternalRepository> _userExternalRepositoryMock;
    private UserExternalService _userExternalService;
    private Fixture _fixture = new();

    [SetUp]
    public void SetUp()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _userExternalRepositoryMock = new Mock<IUserExternalRepository>();
        _userExternalService = new UserExternalService(_userExternalRepositoryMock.Object);
    }

    [Test]
    public async Task UpdateUserTotalPointsUsageAsync_ShouldCallRepositoryMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var usedTokens = 10;

        // Act
        await _userExternalService.UpdateUserTotalPointsUsageAsync(userId, usedTokens);

        // Assert
        _userExternalRepositoryMock.Verify(x => x.UpdateUserTotalPointsUsageAsync(userId, usedTokens), Times.Once);
    }

    [Test]
    public async Task GetUserTotalPointsUsageAsync_ShouldReturnRepositoryResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUsage = 50;
        _userExternalRepositoryMock.Setup(x => x.GetUserTotalPointsUsageAsync(userId)).ReturnsAsync(expectedUsage);

        // Act
        var result = await _userExternalService.GetUserTotalPointsUsageAsync(userId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedUsage));
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnRepositoryResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = _fixture.Create<User>();
        _userExternalRepositoryMock.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userExternalService.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task FindUserByEmailAsync_ShouldReturnRepositoryResult()
    {
        // Arrange
        var email = _fixture.Create<string>();
        var user = _fixture.Create<List<User>>();
        _userExternalRepositoryMock.Setup(x => x.FindUserByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _userExternalService.FindUserByEmailAsync(email);

        // Assert
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task GetUsersByIdsAsync_ShouldReturnRepositoryResult()
    {
        // Arrange
        var userIds = _fixture.Create<List<Guid>>();
        var users = _fixture.Create<List<User>>();
        _userExternalRepositoryMock.Setup(x => x.GetUsersByIdsAsync(userIds)).ReturnsAsync(users);

        // Act
        var result = await _userExternalService.GetUsersByIdsAsync(userIds);

        // Assert
        Assert.That(result, Is.EqualTo(users));
    }

    [Test]
    public async Task GetUsersByIdsWithPaginationAsync_ShouldReturnRepositoryResult()
    {
        // Arrange
        var userIds = _fixture.Create<List<Guid>>();
        var paginationRequest = _fixture.Create<PaginationRequestDto>();
        var users = _fixture.Create<PaginationResponseDto<User>>();
        _userExternalRepositoryMock.Setup(x => x.GetUsersByIdsWithPaginationAsync(userIds, paginationRequest)).ReturnsAsync(users);

        // Act
        var result = await _userExternalService.GetUsersByIdsWithPaginationAsync(userIds, paginationRequest);

        // Assert
        Assert.That(result, Is.EqualTo(users));
    }

    [Test]
    public async Task AddStripeCustomerIdAsync_ShouldCallRepositoryMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerId = _fixture.Create<string>();

        // Act
        await _userExternalService.AddStripeCustomerIdAsync(userId, customerId);

        // Assert
        _userExternalRepositoryMock.Verify(x => x.AddStripeCustomerIdAsync(userId, customerId), Times.Once);
    }

    [Test]
    public async Task RemoveStripeCustomerIdAsync_ShouldCallRepositoryMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _userExternalService.RemoveStripeCustomerIdAsync(userId);

        // Assert
        _userExternalRepositoryMock.Verify(x => x.RemoveStripeCustomerIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task UpdateTotalPurchasedTokensAsync_ShouldCallRepositoryMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var totalPurchasedTokens = 100;

        // Act
        await _userExternalService.UpdateTotalPurchasedTokensAsync(userId, totalPurchasedTokens);

        // Assert
        _userExternalRepositoryMock.Verify(x => x.UpdateTotalPurchasedTokensAsync(userId, totalPurchasedTokens), Times.Once);
    }

    [Test]
    public async Task UpdateAccountPlanAsync_ShouldCallRepositoryMethod()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountPlan = _fixture.Create<AccountPlan>();

        // Act
        await _userExternalService.UpdateAccountPlanAsync(userId, accountPlan);

        // Assert
        _userExternalRepositoryMock.Verify(x => x.UpdateAccountPlanAsync(userId, accountPlan), Times.Once);
    }
}
