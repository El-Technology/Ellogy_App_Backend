using AutoFixture;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using PaymentManager.BLL.Hubs;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.Tests.ServiceTests;
public class WebhookServiceTest : StripeBaseServiceForTests
{
    private Mock<IHubContext<PaymentHub>> _hubContext;
    private Mock<ILogger<WebhookService>> _logger;
    private Mock<IPaymentRepository> _paymentRepository;
    private Mock<IProductCatalogService> _productCatalogService;
    private Mock<ISubscriptionRepository> _subscriptionRepository;
    private Mock<IUserRepository> _userRepository;

    private Mock<WebhookService> _webhookService;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();

        _hubContext = new Mock<IHubContext<PaymentHub>>();
        _logger = new Mock<ILogger<WebhookService>>();
        _paymentRepository = new Mock<IPaymentRepository>();
        _productCatalogService = new Mock<IProductCatalogService>();
        _subscriptionRepository = new Mock<ISubscriptionRepository>();
        _userRepository = new Mock<IUserRepository>();

        _webhookService = new Mock<WebhookService>(
            _hubContext.Object,
            _logger.Object,
            _paymentRepository.Object,
            _productCatalogService.Object,
            _subscriptionRepository.Object,
            _userRepository.Object);

        _sessionService = new Mock<SessionService>();
        _subscriptionService = new Mock<SubscriptionService>();
    }

    [Test]
    public async Task OrderConfirmationAsync_IfPaymentMode_ShouldUpdatePaymentAndUserWallet()
    {
        var numberOfPoints = 1000;

        var session = new Session()
        {
            Id = _fixture.Create<string>(),
            Mode = Constants.PaymentMode,
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.AmountOfPoint, numberOfPoints.ToString() }
            },
            Status = "completed",
            CustomerEmail = "test@gmail.com"
        };

        var payment = _fixture.Create<Payment>();
        payment.UpdatedBallance = false;
        payment.Mode = Constants.PaymentMode;

        _paymentRepository.Setup(x => x.GetPaymentAsync(It.IsAny<string>()))
            .ReturnsAsync(new Payment());


    }

}
