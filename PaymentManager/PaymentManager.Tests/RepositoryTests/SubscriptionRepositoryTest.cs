using AutoFixture;
using Moq;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.Tests.RepositoryTests;

public class SubscriptionRepositoryTest
{
    private Fixture _fixture;
    private Mock<ISubscriptionRepository> _subscriptionRepository;

    [SetUp]
    public void Setup()
    {
        _subscriptionRepository = new Mock<ISubscriptionRepository>();
        _fixture = new Fixture();
    }

    [Test]
    public async Task CreateSubscriptionAsync_ShouldCreatesSubscription()
    {
        // Arrange
        var subscription = _fixture.Create<Subscription>();
        var accountPlan = _fixture.Create<AccountPlan>();
        _subscriptionRepository.Setup(x => x.CreateSubscriptionAsync(It.IsAny<Subscription>(),
            It.IsAny<AccountPlan>())).Returns(Task.CompletedTask);

        // Act
        await _subscriptionRepository.Object.CreateSubscriptionAsync(subscription, accountPlan);

        // Assert
        _subscriptionRepository.Verify(x => x.CreateSubscriptionAsync(It.IsAny<Subscription>(),
            It.IsAny<AccountPlan>()), Times.Once);
    }

    [Test]
    public async Task UpdateSubscriptionAsync_ShouldUpdatesSubscription()
    {
        // Arrange
        var subscription = _fixture.Create<Subscription>();
        var accountPlan = _fixture.Create<AccountPlan>();
        _subscriptionRepository.Setup(x => x.UpdateSubscriptionAsync(It.IsAny<Subscription>(),
            It.IsAny<AccountPlan?>())).Returns(Task.CompletedTask);

        // Act
        await _subscriptionRepository.Object.UpdateSubscriptionAsync(subscription, accountPlan);

        // Assert
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionAsync(It.IsAny<Subscription>(),
            It.IsAny<AccountPlan?>()), Times.Once);
    }

    [Test]
    public async Task UpdateSubscriptionIsCanceledAsync_ShouldUpdatesSubscriptionIsCanceled()
    {
        // Arrange
        var stripeId = _fixture.Create<string>();
        _subscriptionRepository.Setup(x => x.UpdateSubscriptionIsCanceledAsync(It.IsAny<string>(),
            It.IsAny<bool>())).Returns(Task.CompletedTask);

        // Act
        await _subscriptionRepository.Object.UpdateSubscriptionIsCanceledAsync(stripeId, true);

        // Assert
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionIsCanceledAsync(It.IsAny<string>(),
            It.IsAny<bool>()), Times.Once);
    }

    [Test]
    public async Task GetActiveSubscriptionAsync_ShouldReturnsActiveSubscription()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var subscription = _fixture.Create<Subscription>();
        _subscriptionRepository.Setup(x => x.GetActiveSubscriptionAsync(It.IsAny<Guid>()))
            .ReturnsAsync(subscription);

        // Act
        var result = await _subscriptionRepository.Object.GetActiveSubscriptionAsync(userId);

        // Assert
        Assert.That(result, Is.EqualTo(subscription));
    }

    [Test]
    public async Task UpdateSubscriptionStatusAsync_ShouldUpdatesSubscriptionStatus()
    {
        // Arrange
        var stripeId = _fixture.Create<string>();
        var status = SubscriptionStatusEnum.PendingUpgrade;
        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
            It.IsAny<SubscriptionStatusEnum>())).Returns(Task.CompletedTask);

        // Act
        await _subscriptionRepository.Object.UpdateSubscriptionStatusAsync(stripeId, status);

        // Assert
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
            It.IsAny<SubscriptionStatusEnum>()), Times.Once);
    }
}