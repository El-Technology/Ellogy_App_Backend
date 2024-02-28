using AutoFixture;
using AutoMapper;
using Moq;
using Moq.Protected;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Mapping;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.Tests.ServiceTests;

public class PaymentSessionServiceTest : StripeBaseServiceForTests
{
    private Mock<PaymentCustomerService> _paymentCustomerService;
    private Mock<IPaymentRepository> _paymentRepository;
    private Mock<IProductCatalogService> _productCatalogService;
    private Mock<ISubscriptionRepository> _subscriptionRepository;
    private Mock<IUserRepository> _userRepository;
    private Mock<PaymentSessionService> _paymentSessionService;
    private IMapper _mapper;

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

        _userRepository = new Mock<IUserRepository>();
        _subscriptionRepository = new Mock<ISubscriptionRepository>();
        _productCatalogService = new Mock<IProductCatalogService>();
        _paymentRepository = new Mock<IPaymentRepository>();
        _paymentCustomerService = new Mock<PaymentCustomerService>(_userRepository.Object,
            _subscriptionRepository.Object,
            _mapper);

        _paymentSessionService = new Mock<PaymentSessionService>(_paymentRepository.Object,
            _userRepository.Object, _productCatalogService.Object,
            _paymentCustomerService.Object,
            _subscriptionRepository.Object);
    }

    [Test]
    public async Task CreateOneTimePaymentSessionAsync_WhenUserExists_ShouldReturnsPaymentSession()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var createPaymentRequest = _fixture.Create<CreatePaymentRequest>();

        _userRepository.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync(user);

        _paymentSessionService.Protected().Setup<CustomerService>(GET_CUSTOMER_SERVICE).Returns(_customerService.Object);
        _customerService.Setup(x=>x.GetAsync(It.IsAny<string>(), It.IsAny<CustomerGetOptions>(), null, default))
            .ReturnsAsync(new Customer() { InvoiceSettings = new CustomerInvoiceSettings() { DefaultPaymentMethodId = "defaultPaymentMethodId"} });

        // Act
        var result = await _paymentSessionService.Object.CreateOneTimePaymentAsync(user.Id, createPaymentRequest);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.SuccessUrl, Is.EqualTo(createPaymentRequest.SuccessUrl));
        Assert.That(result.CancelUrl, Is.EqualTo(createPaymentRequest.CancelUrl));
        Assert.That(result.Metadata[MetadataConstants.UserId], Is.EqualTo(user.Id.ToString()));
    }

    [Test]
    public async Task CreateOneTimePaymentSessionAsync_WhenUserNotExists_ShouldReturnsNull()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var createPaymentRequest = _fixture.Create<CreatePaymentRequest>();

        _userRepository.Setup(x => x.GetUserByIdAsync(It.Is<Guid>(a => a == user.Id))).ReturnsAsync((User?)null);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
             await _paymentSessionService.Object.CreateOneTimePaymentAsync(user.Id, createPaymentRequest));

        // Assert
        Assert.That(ex, Is.Not.Null);
    }
}