using AutoFixture;
using AutoMapper;
using Moq;
using Moq.Protected;
using PaymentManager.BLL.Interfaces.IHttpServices;
using PaymentManager.BLL.Mapping;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Constants;
using PaymentManager.Common.Dtos;
using PaymentManager.DAL.Interfaces;
using Stripe;
using PaymentMethod = Stripe.PaymentMethod;
using Subscription = Stripe.Subscription;

namespace PaymentManager.Tests.ServiceTests;

public class PaymentCustomerServiceTest : StripeBaseServiceForTests
{
    private IMapper _mapper;
    private Mock<PaymentCustomerService> _paymentCustomerService;
    private Mock<ISubscriptionRepository> _subscriptionRepository;
    private Mock<IUserExternalHttpService> _userExternalHttpService;
    private Mock<IPaymentRepository> _paymentRepository;

    [SetUp]
    public void Setup()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PaymentMethodProfile>();
            cfg.AddProfile<PaymentObjectProfile>();
        });

        _mapper = new Mapper(configuration);

        _invoiceService = new Mock<InvoiceService>();
        _priceService = new Mock<PriceService>();
        _paymentIntentService = new Mock<PaymentIntentService>();
        _subscriptionService = new Mock<SubscriptionService>();
        _customerService = new Mock<CustomerService>();
        _paymentMethodService = new Mock<PaymentMethodService>();

        _userExternalHttpService = new Mock<IUserExternalHttpService>();
        _subscriptionRepository = new Mock<ISubscriptionRepository>();
        _paymentRepository = new Mock<IPaymentRepository>();

        _paymentCustomerService =
            new Mock<PaymentCustomerService>(_subscriptionRepository.Object,
                _mapper, _userExternalHttpService.Object, _paymentRepository.Object);
    }

    [Test]
    public async Task CreateCustomerAsync_WhenUserExists_ShouldCreateCustomer()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        user.StripeCustomerId = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);
        _userExternalHttpService.Setup(x => x.AddStripeCustomerIdAsync(It.Is<Guid>(a => a == user.Id), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        _customerService.Setup(x => x.CreateAsync(It.IsAny<CustomerCreateOptions>(), null, default))
            .ReturnsAsync(new Customer());
        _paymentCustomerService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);

        // Act
        await _paymentCustomerService.Object.CreateCustomerAsync(user.Id);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _userExternalHttpService.Verify(x => x.AddStripeCustomerIdAsync(It.Is<Guid>(a => a == user.Id), It.IsAny<string>()),
            Times.Once);
        _customerService.Verify(x => x.CreateAsync(It.IsAny<CustomerCreateOptions>(), null, default), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_CUSTOMER_SERVICE, Times.Once());
    }

    [Test]
    public void CreateCustomerAsync_WhenUserExistsAndCustomerExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentCustomerService.Object.CreateCustomerAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void CreateCustomerAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(null as UserDto);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.CreateCustomerAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task UpdateCustomerDataAsync_WhenUserExists_ShouldUpdateCustomerData()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        _customerService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateOptions>(), null, default))
            .ReturnsAsync(new Customer());
        _paymentCustomerService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);

        // Act
        await _paymentCustomerService.Object.UpdateCustomerDataAsync(user.Id);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _customerService.Verify(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateOptions>(), null, default), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_CUSTOMER_SERVICE, Times.Once());
    }

    [Test]
    public void UpdateCustomerDataAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(null as UserDto);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.UpdateCustomerDataAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void UpdateCustomerDataAsync_WhenUserExistsAndCustomerNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        user.StripeCustomerId = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.UpdateCustomerDataAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task AddCustomerPaymentMethodAsync_WhenUserExists_ShouldCreateSessionObjectAndReturnIt()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var session = _fixture.Create<CreateSessionRequest>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var result = await _paymentCustomerService.Object.AddCustomerPaymentMethodAsync(user.Id, session);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Mode, Is.EqualTo(Constants.SetupMode));
        Assert.That(result.Metadata[MetadataConstants.UserId], Is.EqualTo(user.Id.ToString()));
    }

    [Test]
    public void AddCustomerPaymentMethodAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.AddCustomerPaymentMethodAsync(user.Id, new CreateSessionRequest()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void AddCustomerPaymentMethodAsync_WhenUserExistsAndCustomerNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        user.StripeCustomerId = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.AddCustomerPaymentMethodAsync(user.Id, new CreateSessionRequest()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task RetrieveCustomerPaymentMethodsAsync_WhenUserExists_ShouldReturnListOfPaymentMethods()
    {
        // Arrange
        var paginationRequestDto = new StripePaginationRequestDto();
        var user = _fixture.Create<UserDto>();

        var paymentMethods = new List<PaymentMethod>
        {
            new(),
            new()
        };

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);
        _paymentCustomerService.Protected().Setup<PaymentMethodService>(GET_PAYMENT_METHOD_SERVICE)
            .Returns(_paymentMethodService.Object);
        _paymentMethodService.Setup(x => x.ListAsync(It.IsAny<PaymentMethodListOptions>(), null, default))
            .ReturnsAsync(new StripeList<PaymentMethod> { Data = paymentMethods });

        // Act
        var result =
            await _paymentCustomerService.Object.RetrieveCustomerPaymentMethodsAsync(user.Id, paginationRequestDto);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_PAYMENT_METHOD_SERVICE, Times.Once());
        _paymentMethodService.Verify(x => x.ListAsync(It.IsAny<PaymentMethodListOptions>(), null, default), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.HasMore, Is.False);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Count, Is.EqualTo(paymentMethods.Count));
    }

    [Test]
    public void RetrieveCustomerPaymentMethodsAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.RetrieveCustomerPaymentMethodsAsync(user.Id,
                new StripePaginationRequestDto()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void RetrieveCustomerPaymentMethodsAsync_WhenUserExistsAndCustomerNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        user.StripeCustomerId = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.RetrieveCustomerPaymentMethodsAsync(user.Id,
                new StripePaginationRequestDto()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task SetDefaultPaymentMethodAsync_WhenUserExists_ShouldSetDefaultPaymentMethod()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var paymentMethodId = _fixture.Create<string>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);
        _paymentCustomerService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);
        _customerService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateOptions>(), null, default))
            .ReturnsAsync(new Customer());
        _customerService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default))
            .ReturnsAsync(new Customer
            { Subscriptions = new StripeList<Subscription> { Data = new List<Subscription> { new() } } });
        _paymentCustomerService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);

        // Act
        await _paymentCustomerService.Object.SetDefaultPaymentMethodAsync(user.Id, paymentMethodId);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_CUSTOMER_SERVICE, Times.AtMost(2));
        _customerService.Verify(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateOptions>(), null, default), Times.Once);
        _customerService.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default),
            Times.Once);
        _paymentCustomerService.Protected().Verify(GET_SUBSCRIPTION_SERVICE, Times.Once());
    }

    [Test]
    public void SetDefaultPaymentMethodAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.SetDefaultPaymentMethodAsync(user.Id, _fixture.Create<string>()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void SetDefaultPaymentMethodAsync_WhenUserExistsAndCustomerNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        user.StripeCustomerId = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.SetDefaultPaymentMethodAsync(user.Id, _fixture.Create<string>()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void
        SetDefaultPaymentMethodAsync_WhenUserExistsAndCustomerExistsAndSubscriptionNotExists_ShouldNotThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var paymentMethodId = _fixture.Create<string>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);
        _paymentCustomerService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);
        _customerService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<CustomerUpdateOptions>(), null, default))
            .ReturnsAsync(new Customer());
        _customerService.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default))
            .ReturnsAsync(new Customer
            { Subscriptions = new StripeList<Subscription> { Data = new List<Subscription>() } });

        // Act + Assert
        Assert.DoesNotThrowAsync(async () =>
            await _paymentCustomerService.Object.SetDefaultPaymentMethodAsync(user.Id, paymentMethodId));
    }

    [Test]
    public async Task GetCustomerPaymentsAsync_WhenUserExists_ShouldReturnListOfPayments()
    {
        // Arrange
        var paginationRequestDto = new StripePaginationRequestDto();
        var user = _fixture.Create<UserDto>();

        var paymentIntents = new StripeList<PaymentIntent>
        {
            Data = new List<PaymentIntent>
            {
                new(),
                new()
            }
        };

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);
        _paymentCustomerService.Protected().Setup<PaymentIntentService>(GET_PAYMENT_INTENT_SERVICE)
            .Returns(_paymentIntentService.Object);
        _paymentIntentService.Setup(x => x.ListAsync(It.IsAny<PaymentIntentListOptions>(), null, default))
            .ReturnsAsync(paymentIntents);

        // Act
        var result = await _paymentCustomerService.Object.GetCustomerPaymentsAsync(user.Id, paginationRequestDto);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_PAYMENT_INTENT_SERVICE, Times.Once());
        _paymentIntentService.Verify(x => x.ListAsync(It.IsAny<PaymentIntentListOptions>(), null, default), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.HasMore, Is.False);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data.Count, Is.EqualTo(paymentIntents.Data.Count));
    }

    [Test]
    public void GetCustomerPaymentsAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.GetCustomerPaymentsAsync(user.Id, new StripePaginationRequestDto()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void GetCustomerPaymentsAsync_WhenUserExistsAndCustomerNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        user.StripeCustomerId = null;

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.GetCustomerPaymentsAsync(user.Id, new StripePaginationRequestDto()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task GetActiveSubscriptionAsync_WhenUserExists_ShouldReturnActiveSubscription()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();
        var subscription = new DAL.Models.Subscription();

        _subscriptionRepository.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(subscription);

        // Act
        var result = await _paymentCustomerService.Object.GetActiveSubscriptionAsync(user.Id);

        // Assert
        _subscriptionRepository.Verify(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(subscription));
    }

    [Test]
    public async Task GetActiveSubscription_WhenActiveSubscriptionNotExists_ShouldReturnNull()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _subscriptionRepository.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((DAL.Models.Subscription?)null);

        // Act
        var result = await _paymentCustomerService.Object.GetActiveSubscriptionAsync(user.Id);

        // Assert
        _subscriptionRepository.Verify(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpgradeSubscriptionPreviewAsync_WhenUserExists_ShouldReturnUpgradeSubscriptionPreview()
    {
        // Arrange
        var price = 1000;
        var priceInCents = 100000;
        var user = _fixture.Create<UserDto>();
        var newPriceId = _fixture.Create<string>();

        _subscriptionRepository.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync(new DAL.Models.Subscription());
        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);
        _paymentCustomerService.Protected().Setup<PriceService>(GET_PRICE_SERVICE).Returns(_priceService.Object);
        _priceService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new Price { UnitAmount = price });
        _paymentCustomerService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);
        _subscriptionService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default)).ReturnsAsync(
            new Subscription
            { Items = new StripeList<SubscriptionItem> { Data = new List<SubscriptionItem> { new() } } });
        _paymentCustomerService.Protected().Setup<InvoiceService>(GET_INVOICE_SERVICE).Returns(_invoiceService.Object);
        _invoiceService.Setup(x => x.UpcomingAsync(It.IsAny<UpcomingInvoiceOptions>(), null, default))
            .ReturnsAsync(new Invoice { AmountDue = priceInCents });

        // Act
        var result = await _paymentCustomerService.Object.UpgradeSubscriptionPreviewAsync(user.Id, newPriceId);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_PRICE_SERVICE, Times.Once());
        _priceService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_SUBSCRIPTION_SERVICE, Times.Once());
        _invoiceService.Verify(x => x.UpcomingAsync(It.IsAny<UpcomingInvoiceOptions>(), null, default), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_INVOICE_SERVICE, Times.Once());
        _subscriptionService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _subscriptionRepository.Verify(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);

        Assert.That(result, Is.EqualTo(price));
    }

    [Test]
    public void UpgradeSubscriptionPreviewAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentCustomerService.Object.UpgradeSubscriptionPreviewAsync(user.Id, _fixture.Create<string>()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void UpgradeSubscriptionPreviewAsync_WhenActiveSubscriptionNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _subscriptionRepository.Setup(x => x.GetActiveSubscriptionAsync(It.Is<Guid>(a => a == user.Id)))
            .ReturnsAsync((DAL.Models.Subscription?)null);

        // Act
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _paymentCustomerService.Object.UpgradeSubscriptionPreviewAsync(user.Id, _fixture.Create<string>()));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task DetachPaymentMethodAsync_WhenPaymentMethodExists_ShouldDetachPaymentMethod()
    {
        // Arrange
        var paymentMethodId = _fixture.Create<string>();

        _paymentCustomerService.Protected().Setup<PaymentMethodService>(GET_PAYMENT_METHOD_SERVICE)
            .Returns(_paymentMethodService.Object);
        _paymentMethodService.Setup(x => x.DetachAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new PaymentMethod());

        // Act
        await _paymentCustomerService.Object.DetachPaymentMethodAsync(paymentMethodId);

        // Assert
        _paymentCustomerService.Protected().Verify(GET_PAYMENT_METHOD_SERVICE, Times.Once());
        _paymentMethodService.Verify(x => x.DetachAsync(It.IsAny<string>(), null, null, default), Times.Once);
    }

    [Test]
    public async Task DeleteCustomerAsync_WhenUserExists_ShouldDeleteCustomer()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);
        _paymentCustomerService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE)
            .Returns(_customerService.Object);
        _customerService.Setup(x => x.DeleteAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(new Customer());

        // Act
        await _paymentCustomerService.Object.DeleteCustomerAsync(user.Id);

        // Assert
        _userExternalHttpService.Verify(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id)), Times.Once);
        _paymentCustomerService.Protected().Verify(GET_CUSTOMER_SERVICE, Times.Once());
        _customerService.Verify(x => x.DeleteAsync(It.IsAny<string>(), null, null, default), Times.Once);
    }

    [Test]
    public void DeleteCustomerAsync_WhenUserNotExists_ShouldThrowException()
    {
        // Arrange
        var user = _fixture.Create<UserDto>();

        _userExternalHttpService.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((UserDto?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _paymentCustomerService.Object.DeleteCustomerAsync(user.Id));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }
}