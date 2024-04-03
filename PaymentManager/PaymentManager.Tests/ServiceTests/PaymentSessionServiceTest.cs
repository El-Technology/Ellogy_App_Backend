using AutoFixture;
using Moq;
using Moq.Protected;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Interfaces.IHttpServices;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Subscription = Stripe.Subscription;

namespace PaymentManager.Tests.ServiceTests;

public class PaymentSessionServiceTest : StripeBaseServiceForTests
{
    private Mock<IPaymentCustomerService> _paymentCustomerService;
    private Mock<IPaymentRepository> _paymentRepository;
    private Mock<IProductCatalogService> _productCatalogService;
    private Mock<ISubscriptionRepository> _subscriptionRepository;
    private Mock<IUserExternalHttpService> _userExternalHttpService;
    private Mock<PaymentSessionService> _paymentSessionService;

    [SetUp]
    public void Setup()
    {
        _invoiceService = new Mock<InvoiceService>();
        _priceService = new Mock<PriceService>();
        _paymentIntentService = new Mock<PaymentIntentService>();
        _subscriptionService = new Mock<SubscriptionService>();
        _customerService = new Mock<CustomerService>();
        _paymentMethodService = new Mock<PaymentMethodService>();

        _userExternalHttpService = new Mock<IUserExternalHttpService>();
        _subscriptionRepository = new Mock<ISubscriptionRepository>();
        _productCatalogService = new Mock<IProductCatalogService>();
        _paymentRepository = new Mock<IPaymentRepository>();
        _paymentCustomerService = new Mock<IPaymentCustomerService>();

        _paymentSessionService = new Mock<PaymentSessionService>(
            _paymentRepository.Object,
            _productCatalogService.Object,
            _paymentCustomerService.Object,
            _subscriptionRepository.Object,
            _userExternalHttpService.Object);
    }

    [Test]
    public async Task CreateOneTimePaymentSessionAsync_WhenUserExists_ShouldReturnsPaymentSession()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var createPaymentRequest = _fixture.Create<CreatePaymentRequest>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        _paymentSessionService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);

        _customerService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default))
            .ReturnsAsync(new Customer() { InvoiceSettings = new CustomerInvoiceSettings() { DefaultPaymentMethodId = "defaultPaymentMethodId" } });

        // Act
        var result = await _paymentSessionService.Object.CreateOneTimePaymentAsync(user.Id, createPaymentRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.SuccessUrl, Is.EqualTo(createPaymentRequest.SuccessUrl));
        Assert.That(result.CancelUrl, Is.EqualTo(createPaymentRequest.CancelUrl));
        Assert.That(result.Metadata[MetadataConstants.UserId], Is.EqualTo(user.Id.ToString()));
    }

    [Test]
    public void CreateOneTimePaymentSessionAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var createPaymentRequest = _fixture.Create<CreatePaymentRequest>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
             await _paymentSessionService.Object.CreateOneTimePaymentAsync(user.Id, createPaymentRequest));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task GetUserBalanceAsync_WhenWalletExists_ShouldReturnsUserBalance()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var wallet = _fixture.Create<Wallet>();

        _paymentRepository.Setup(x => x.GetUserWalletAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(wallet);

        // Act
        var result = await _paymentSessionService.Object.GetUserBalanceAsync(user.Id);

        // Assert
        Assert.That(result, Is.EqualTo(wallet.Balance));
    }

    [Test]
    public void GetUserBalanceAsync_WhenWalletNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _paymentRepository.Setup(x => x.GetUserWalletAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((Wallet?)null);

        // Act
        var ex = Assert.ThrowsAsync<NullReferenceException>(async () =>
            await _paymentSessionService.Object.GetUserBalanceAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task CreateFreeSubscriptionAsync_WhenUserExists_ShouldReturnsSubscription()
    {
        // Arrange
        var signalRModel = _fixture.Create<SignalRModel>();
        var user = _fixture.Create<UserDto>();
        user.AccountPlan = null;
        var productName = AccountPlan.Free.ToString();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(user);

        _paymentSessionService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);

        _customerService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default))
            .ReturnsAsync(new Customer() { InvoiceSettings = new CustomerInvoiceSettings() { DefaultPaymentMethodId = "defaultPaymentMethodId" } });

        _productCatalogService.Setup(x => x.GetProductByNameAsync(It.Is<string>(a => a.Equals(productName))))
            .ReturnsAsync(new ProductModel() { Name = productName });

        // Act
        var result = await _paymentSessionService.Object.CreateFreeSubscriptionAsync(signalRModel, user.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Metadata[MetadataConstants.UserId], Is.EqualTo(user.Id.ToString()));
    }

    [Test]
    public void CreateFreeSubscriptionAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var signalRModel = _fixture.Create<SignalRModel>();
        var user = _fixture.Create<UserDto>();
        user.AccountPlan = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                   await _paymentSessionService.Object.CreateFreeSubscriptionAsync(signalRModel, user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void CreateFreeSubscriptionAsync_WhenUserHasAccountPlan_ShouldThrowException()
    {
        // Arrange
        var signalRModel = _fixture.Create<SignalRModel>();
        var user = _fixture.Create<UserDto>();
        user.AccountPlan = _fixture.Create<AccountPlan>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
                          await _paymentSessionService.Object.CreateFreeSubscriptionAsync(signalRModel, user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task CancelSubscriptionAsync_UserIsAbleToDoThis_ShouldCancelSubscription()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var subscriptionId = _fixture.Create<string>();
        var productId = _fixture.Create<string>();
        var productName = $"{AccountPlan.Basic}/otherLanguageTranslate";

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(user);

        _paymentSessionService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);

        _customerService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default))
            .ReturnsAsync(new Customer
            {
                Subscriptions = new StripeList<Subscription>
                {
                    Data = new List<Subscription>
                    {
                        new Subscription
                        {
                            Id = subscriptionId,
                            Items = new StripeList<SubscriptionItem>
                            {
                                Data = new List<SubscriptionItem>
                                {
                                    new SubscriptionItem { Plan = new Plan(){ ProductId = productId } }
                                }
                            }
                        }
                    }
                }
            });

        _productCatalogService.Setup(x => x.GetProductAsync(It.Is<string>(a => a.Equals(productId))))
            .ReturnsAsync(new ProductModel() { Name = productName });

        _paymentSessionService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);

        _subscriptionService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(), null, default))
            .ReturnsAsync(new Subscription());

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(), It.IsAny<SubscriptionStatusEnum>()))
            .Returns(Task.CompletedTask);

        // Act
        await _paymentSessionService.Object.CancelSubscriptionAsync(user.Id);

        // Assert
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionStatusAsync(subscriptionId, SubscriptionStatusEnum.PendingCancellation), Times.Once);
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(user.Id), Times.Once);
        _paymentSessionService.Protected().Verify<CustomerService>(GET_CUSTOMER_SERVICE, Times.Once());
        _customerService.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default), Times.Once);
        _productCatalogService.Verify(x => x.GetProductAsync(productId), Times.Once);
        _paymentSessionService.Protected().Verify<SubscriptionService>(GET_SUBSCRIPTION_SERVICE, Times.Once());
        _subscriptionService.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(), null, default), Times.Once);
    }

    [Test]
    public void CancelSubscriptionAsync_UserIsNotAbleToDoThis_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        user.StripeCustomerId = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                          await _paymentSessionService.Object.CancelSubscriptionAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void CancelSubscriptionAsync_UserIsNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                          await _paymentSessionService.Object.CancelSubscriptionAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task UpgradeSubscriptionAsync_WhenUserExists_ShouldUpgradeSubscription()
    {
        // Arrange
        var activeSubscription = _fixture.Create<DAL.Models.Subscription>();
        activeSubscription.IsCanceled = false;
        activeSubscription.Price = 0;
        var newPriceId = "newPriceId";
        var newPriceAmount = 1000;

        var user = _fixture.Create<UserDto>();

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(activeSubscription);

        _paymentSessionService.Protected().Setup<PriceService>(GET_PRICE_SERVICE)
            .Returns(_priceService.Object);

        _priceService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new Price() { UnitAmountDecimal = newPriceAmount });

        _paymentSessionService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);

        _subscriptionService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new Subscription()
            {
                Items = new StripeList<SubscriptionItem>
                {
                    Data = new List<SubscriptionItem>
                    {
                        new SubscriptionItem { Price = new Price() { UnitAmountDecimal = activeSubscription.Price } }
                    }
                }
            });

        _subscriptionService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(), null, default))
            .ReturnsAsync(new Subscription());

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(), It.IsAny<SubscriptionStatusEnum>()));

        // Act
        await _paymentSessionService.Object.UpgradeSubscriptionAsync(user.Id, newPriceId);

        // Assert
        _paymentCustomerService.Verify(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _paymentSessionService.Protected().Verify<PriceService>(GET_PRICE_SERVICE, Times.Once());
        _priceService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _paymentSessionService.Protected().Verify<SubscriptionService>(GET_SUBSCRIPTION_SERVICE, Times.AtMost(2));
        _subscriptionService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _subscriptionService.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(), null, default), Times.Once);
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(), It.IsAny<SubscriptionStatusEnum>()), Times.Once);
    }

    [Test]
    public void UpgradeSubscriptionAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((DAL.Models.Subscription?)null);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentSessionService.Object.UpgradeSubscriptionAsync(user.Id, "newPriceId"));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void UpgradeSubscriptionAsync_WhenUserHasCanceledSubscription_ShouldThrowException()
    {
        // Arrange
        var activeSubscription = _fixture.Create<DAL.Models.Subscription>();
        activeSubscription.IsCanceled = true;
        var user = _fixture.Create<UserDto>();

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(activeSubscription);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentSessionService.Object.UpgradeSubscriptionAsync(user.Id, "newPriceId"));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void UpgradeSubscriptionAsync_WhenUserHasNoActiveSubscription_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((DAL.Models.Subscription?)null);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentSessionService.Object.UpgradeSubscriptionAsync(user.Id, "newPriceId"));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task DowngradeSubscriptionAsync_WhenUserIsAbleToDo_ShouldDowngradeSubscription()
    {
        // Arrange
        var activeSubscription = _fixture.Create<DAL.Models.Subscription>();
        activeSubscription.IsCanceled = false;
        activeSubscription.Price = 1000;
        var newPriceId = "newPriceId";
        var newPriceAmount = 0;

        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(user);

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(activeSubscription);

        _paymentSessionService.Protected().Setup<PriceService>(GET_PRICE_SERVICE)
            .Returns(_priceService.Object);

        _priceService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new Price() { UnitAmountDecimal = newPriceAmount });

        _paymentSessionService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);

        _subscriptionService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new Subscription()
            {
                Items = new StripeList<SubscriptionItem>
                {
                    Data = new List<SubscriptionItem>
                    {
                        new SubscriptionItem { Price = new Price() { UnitAmountDecimal = activeSubscription.Price } }
                    }
                }
            });

        _subscriptionService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(), null, default))
            .ReturnsAsync(new Subscription());

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(), It.IsAny<SubscriptionStatusEnum>()));

        // Act
        await _paymentSessionService.Object.DowngradeSubscriptionAsync(user.Id, newPriceId);

        // Assert
        _paymentCustomerService.Verify(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _paymentSessionService.Protected().Verify<PriceService>(GET_PRICE_SERVICE, Times.Once());
        _priceService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _paymentSessionService.Protected().Verify<SubscriptionService>(GET_SUBSCRIPTION_SERVICE, Times.AtMost(2));
        _subscriptionService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _subscriptionService.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(), null, default), Times.Once);
    }

    [Test]
    public void DowngradeSubscriptionAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((DAL.Models.Subscription?)null);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentSessionService.Object.DowngradeSubscriptionAsync(user.Id, "newPriceId"));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void DowngradeSubscriptionAsync_WhenUserHasCanceledSubscription_ShouldThrowException()
    {
        // Arrange
        var activeSubscription = _fixture.Create<DAL.Models.Subscription>();
        activeSubscription.IsCanceled = true;
        var user = _fixture.Create<UserDto>();

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(activeSubscription);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentSessionService.Object.DowngradeSubscriptionAsync(user.Id, "newPriceId"));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void DowngradeSubscriptionAsync_WhenUserHasNoActiveSubscription_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _paymentCustomerService.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((DAL.Models.Subscription?)null);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
                   await _paymentSessionService.Object.DowngradeSubscriptionAsync(user.Id, "newPriceId"));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }
}