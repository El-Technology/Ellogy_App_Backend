using AutoFixture;
using Moq;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Services;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe.Checkout;

namespace PaymentManager.Tests.ServiceTests;

public class PaymentSessionServiceTest : StripeBaseServiceForTests
{
    private readonly Mock<PaymentCustomerService> _paymentCustomerService;
    private readonly Mock<IPaymentRepository> _paymentRepository;
    private readonly Mock<IProductCatalogService> _productCatalogService;
    private readonly Mock<ISubscriptionRepository> _subscriptionRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private Mock<PaymentSessionService> _paymentSessionService;

    [SetUp]
    public void Setup()
    {
        _userRepository = new Mock<IUserRepository>();
        _subscriptionRepository = new Mock<ISubscriptionRepository>();
        _productCatalogService = new Mock<IProductCatalogService>();
        _paymentRepository = new Mock<IPaymentRepository>();
        _paymentCustomerService = new Mock<PaymentCustomerService>(_userRepository.Object,
            _subscriptionRepository.Object, _productCatalogService.Object);

        _paymentSessionService = new Mock<PaymentSessionService>(_paymentRepository.Object);
    }

    [Test]
    public async Task CreateOneTimePaymentSessionAsync_WhenUserExists_ShouldReturnsPaymentSession()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var paymentSession = _fixture.Create<SessionCreateOptions>();
        _userRepository.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == userId))).ReturnsAsync(user);

        // Act
        var result = await _paymentSessionService.Object.CreateOneTimePaymentSessionAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(paymentSession));
    }
}