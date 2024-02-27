using AutoFixture;
using Moq;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.Tests.RepositoryTests;

public class UserRepositoryTest
{
    private Fixture _fixture;
    private Mock<IUserRepository> _userRepository;

    [SetUp]
    public void Setup()
    {
        _userRepository = new Mock<IUserRepository>();
        _fixture = new Fixture();
    }

    [Test]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnsUser()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var result = await _userRepository.Object.GetUserByIdAsync(user.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task GetUserByIdAsync_WhenUserNotExists_ShouldReturnsNullUser()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((User?)null);

        // Act
        var result = await _userRepository.Object.GetUserByIdAsync(user.Id);

        // Assert
        Assert.That(result, Is.Null);
        Assert.That(result, Is.Not.EqualTo(user));
    }

    [Test]
    public async Task GetAllUsersAsync_WhenUsersExists_ShouldReturnsUsers()
    {
        // Arrange
        var users = _fixture.CreateMany<User>().ToList();
        _userRepository.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userRepository.Object.GetAllUsersAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(users));
    }

    [Test]
    public async Task GetAllUsersAsync_WhenUsersNotExists_ShouldReturnsEmptyList()
    {
        // Arrange
        var users = new List<User>();
        _userRepository.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userRepository.Object.GetAllUsersAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(users));
    }

    [Test]
    [TestCase(123)]
    [TestCase(0)]
    [TestCase(-434)]
    public async Task UpdateTotalPurchasedTokensAsync_WhenUserExists_ShouldUpdateTotalPurchasedTokens(
        int purchasedTokens)
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.UpdateTotalPurchasedTokensAsync(It.Is<Guid>(a => a == user.Id),
            It.Is<int>(a => a == purchasedTokens))).Returns(Task.CompletedTask);

        // Act
        await _userRepository.Object.UpdateTotalPurchasedTokensAsync(user.Id, purchasedTokens);

        // Assert
        _userRepository.Verify(x => x.UpdateTotalPurchasedTokensAsync(It.Is<Guid>(a => a == user.Id),
            It.Is<int>(a => a == purchasedTokens)), Times.Once);
    }

    [Test]
    public async Task AddStripeCustomerIdAsync_WhenUserExists_ShouldAddStripeCustomerId()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var customerId = _fixture.Create<string>();
        _userRepository.Setup(x => x.AddStripeCustomerIdAsync(It.Is<Guid>(a => a == user.Id),
            It.Is<string>(a => a == customerId))).Returns(Task.CompletedTask);

        // Act
        await _userRepository.Object.AddStripeCustomerIdAsync(user.Id, customerId);

        // Assert
        _userRepository.Verify(x => x.AddStripeCustomerIdAsync(It.Is<Guid>(a => a == user.Id),
            It.Is<string>(a => a == customerId)), Times.Once);
    }

    [Test]
    public async Task UpdateAccountPlanAsync_WhenUserExists_ShouldUpdateAccountPlan()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var accountPlan = _fixture.Create<AccountPlan>();
        _userRepository.Setup(x => x.UpdateAccountPlanAsync(It.Is<Guid>(a => a == user.Id),
            It.Is<AccountPlan>(a => a == accountPlan))).Returns(Task.CompletedTask);

        // Act
        await _userRepository.Object.UpdateAccountPlanAsync(user.Id, accountPlan);

        // Assert
        _userRepository.Verify(x => x.UpdateAccountPlanAsync(It.Is<Guid>(a => a == user.Id),
            It.Is<AccountPlan>(a => a == accountPlan)), Times.Once);
    }

    [Test]
    public async Task RemoveStripeCustomerIdAsync_WhenUserExists_ShouldRemoveStripeCustomerId()
    {
        // Arrange
        var user = _fixture.Create<User>();
        _userRepository.Setup(x => x.RemoveStripeCustomerIdAsync(It.Is<Guid>(a => a == user.Id)))
            .Returns(Task.CompletedTask);

        // Act
        await _userRepository.Object.RemoveStripeCustomerIdAsync(user.Id);

        // Assert
        _userRepository.Verify(x => x.RemoveStripeCustomerIdAsync(It.Is<Guid>(a => a == user.Id)),
            Times.Once);
    }
}