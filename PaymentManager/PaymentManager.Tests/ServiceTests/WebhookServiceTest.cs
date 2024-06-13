using AutoFixture;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PaymentManager.BLL.Hubs;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Interfaces.IHttpServices;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Enums;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;
using Stripe;
using Stripe.Checkout;
using Subscription = Stripe.Subscription;

namespace PaymentManager.Tests.ServiceTests;
public class WebhookServiceTest : StripeBaseServiceForTests
{
    private Mock<IHubContext<PaymentHub>> _hubContext;
    private Mock<ILogger<WebhookService>> _logger;
    private Mock<IPaymentRepository> _paymentRepository;
    private Mock<IProductCatalogService> _productCatalogService;
    private Mock<ISubscriptionRepository> _subscriptionRepository;
    private Mock<IUserExternalHttpService> _userExternalHttpService;
    private Mock<IPaymentCustomerService> _paymentCustomerService;

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
        _userExternalHttpService = new Mock<IUserExternalHttpService>();
        _paymentCustomerService = new Mock<IPaymentCustomerService>();

        _webhookService = new Mock<WebhookService>(
            _paymentRepository.Object,
            _subscriptionRepository.Object,
            _logger.Object,
            _productCatalogService.Object,
            _hubContext.Object,
            _userExternalHttpService.Object,
            _paymentCustomerService.Object);

        _invoiceService = new Mock<InvoiceService>();
        _sessionService = new Mock<SessionService>();
        _subscriptionService = new Mock<SubscriptionService>();
    }

    [Test]
    public async Task OrderConfirmationAsync_IfPaymentMode_ShouldUpdatePaymentAndUserWallet()
    {
        //Arrange
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
            CustomerEmail = "test@gmail.com",
            PaymentIntentId = _fixture.Create<string>()
        };

        var payment = _fixture.Create<Payment>();
        payment.UpdatedBallance = false;
        payment.Mode = Constants.PaymentMode;

        _paymentRepository.Setup(x => x.GetPaymentAsync(It.IsAny<string>()))
            .ReturnsAsync(payment);

        _paymentRepository.Setup(x => x.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<int>()));

        _userExternalHttpService.Setup(x => x.UpdateTotalPurchasedTokensAsync(It.IsAny<Guid>(), It.IsAny<int>()));

        //Act
        await _webhookService.Object.OrderConfirmationAsync(session);

        //Assert

        _paymentRepository.Verify(x => x.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Once);
        _userExternalHttpService.Verify(x => x.UpdateTotalPurchasedTokensAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
        _paymentRepository.Verify(x => x.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Once);
    }

    [Test]
    public async Task ExpireSessionAsync_IfPaymentModeIsNotExpired_ShouldExpireAndUpdatePayment()
    {
        //Arrange
        var session = new Session()
        {
            Id = _fixture.Create<string>(),
            Mode = Constants.PaymentMode,
            Status = "open",
            PaymentIntentId = _fixture.Create<string>(),
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.AmountOfPoint, "1000" }
            }
        };

        var payment = _fixture.Create<Payment>();
        payment.Status = "open";
        payment.UpdatedBallance = false;
        payment.Mode = Constants.PaymentMode;

        _paymentRepository.Setup(x => x.GetPaymentAsync(It.IsAny<string>()))
            .ReturnsAsync(payment);

        _paymentRepository.Setup(x => x.UpdatePaymentAsync(It.IsAny<Payment>()));

        _webhookService.Protected().Setup<SessionService>(GET_SESSION_SERVICE)
            .Returns(_sessionService.Object);

        _sessionService.Setup(x => x.ExpireAsync(It.IsAny<string>(), null, null, default));

        //Act
        await _webhookService.Object.ExpireSessionAsync(session);

        //Assert
        _paymentRepository.Verify(x => x.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Once);
        _sessionService.Verify(x => x.ExpireAsync(It.IsAny<string>(), null, null, default), Times.Once);
    }

    [Test]
    public async Task ExpireSessionAsync_IfPaymentModeIsExpired_ShouldNotExpireAndUpdatePayment()
    {
        //Arrange
        var session = new Session()
        {
            Id = _fixture.Create<string>(),
            Mode = Constants.PaymentMode,
            Status = "expired",
            PaymentIntentId = _fixture.Create<string>(),
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.AmountOfPoint, "1000" }
            }
        };

        var payment = _fixture.Create<Payment>();
        payment.Status = "expired";
        payment.UpdatedBallance = false;
        payment.Mode = Constants.PaymentMode;

        _paymentRepository.Setup(x => x.GetPaymentAsync(It.IsAny<string>()))
            .ReturnsAsync(payment);

        _paymentRepository.Setup(x => x.UpdatePaymentAsync(It.IsAny<Payment>()));

        _webhookService.Protected().Setup<SessionService>(GET_SESSION_SERVICE)
            .Returns(_sessionService.Object);

        _sessionService.Setup(x => x.ExpireAsync(It.IsAny<string>(), null, null, default));

        //Act
        await _webhookService.Object.ExpireSessionAsync(session);

        //Assert
        _paymentRepository.Verify(x => x.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Never);
        _sessionService.Verify(x => x.ExpireAsync(It.IsAny<string>(), null, null, default), Times.Never);
    }

    [Test]
    public async Task UpdateSubscriptionAsync_IfSubscriptionCancelAtPeriodEnd_ShouldUpdateSubscription()
    {
        //Arrange
        var subscription = new Subscription()
        {
            Id = _fixture.Create<string>(),
            Status = "canceled",
            CancelAtPeriodEnd = true,
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.UserId, _fixture.Create<Guid>().ToString() }
            }
        };

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionIsCanceledAsync(It.IsAny<string>(),
            It.Is<bool>(a => a == true)));

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
            It.IsAny<SubscriptionStatusEnum>()));

        //Act
        await _webhookService.Object.UpdateSubscriptionAsync(subscription);

        //Assert
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionIsCanceledAsync(It.IsAny<string>(),
            It.Is<bool>(a => a == true)), Times.Once);

        _subscriptionRepository.Verify(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
            It.IsAny<SubscriptionStatusEnum>()), Times.Once);
    }

    [Test]
    public async Task DeleteSubscriptionAsync_ShouldDeleteSubscription()
    {
        // Arrange
        var productId = _fixture.Create<string>();

        var subscription = new Stripe.Subscription()
        {
            Id = _fixture.Create<string>(),
            Status = "canceled",
            CancelAtPeriodEnd = true,
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.UserId, _fixture.Create<Guid>().ToString() }
            },
            Items = new StripeList<SubscriptionItem>()
            {
                Data = new List<SubscriptionItem>()
                {
                    new SubscriptionItem()
                    {
                        Plan = new Plan()
                        {
                            ProductId = productId
                        }
                    }
                }
            }
        };

        var productModel = new ProductModel()
        {
            Name = "Test Product",
            Price = 10
        };

        _productCatalogService.Setup(x => x.GetProductAsync(productId))
            .ReturnsAsync(productModel);

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionAsync(It.IsAny<DAL.Models.Subscription>(), null));

        // Act
        await _webhookService.Object.DeleteSubscriptionAsync(subscription);

        // Assert
        _productCatalogService.Verify(x => x.GetProductAsync(productId), Times.Once);
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionAsync(It.IsAny<DAL.Models.Subscription>(), null), Times.Once);
    }

    [Test]
    public async Task InvoiceFailedHandleAsync_ShouldVoidUpcomingInvoiceAndApplyFreeSubscriptionPlan()
    {
        var userId = _fixture.Create<Guid>();
        var invoice = new Invoice()
        {
            SubscriptionId = _fixture.Create<string>(),
            PeriodEnd = DateTime.UtcNow,
            WebhooksDeliveredAt = DateTime.UtcNow.AddDays(1)
        };

        var freeProduct = new ProductModel()
        {
            Name = "Free",
            Price = 0,
            PriceId = _fixture.Create<string>()
        };

        var currentSubscription = new Subscription()
        {
            Items = new StripeList<SubscriptionItem>()
            {
                Data = new List<SubscriptionItem>()
                {
                    new SubscriptionItem()
                    {
                        Plan = new Plan()
                        {
                            ProductId = _fixture.Create<string>()
                        }
                    }
                }
            },
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.UserId, userId.ToString() }
            }
        };
        var newSubscription = new Subscription();

        _webhookService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);

        _webhookService.Protected().Setup<InvoiceService>(GET_INVOICE_SERVICE)
            .Returns(_invoiceService.Object);

        _subscriptionService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(currentSubscription);

        _productCatalogService.Setup(x => x.GetProductByNameAsync(It.Is<string>(a =>
            a.Equals(AccountPlan.Free.ToString())))).ReturnsAsync(freeProduct);

        _subscriptionService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(),
            null, default)).ReturnsAsync(newSubscription);

        _invoiceService.Setup(x => x.VoidInvoiceAsync(It.IsAny<string>(), null, null, default));

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionAsync(It.IsAny<DAL.Models.Subscription>(), null));

        _paymentRepository.Setup(x => x.CreatePaymentAsync(It.IsAny<Payment>()));

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
                       It.IsAny<SubscriptionStatusEnum>()));

        // Act
        await _webhookService.Object.InvoiceFailedHandleAsync(invoice);

        //Assert
        _subscriptionService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _productCatalogService.Verify(x => x.GetProductByNameAsync(It.Is<string>(a =>
            a.Equals(AccountPlan.Free.ToString()))), Times.Once);
        _subscriptionService.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(),
            null, default), Times.Once);
        _invoiceService.Verify(x => x.VoidInvoiceAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _paymentRepository.Verify(x => x.CreatePaymentAsync(It.IsAny<Payment>()), Times.Once);
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
                       It.IsAny<SubscriptionStatusEnum>()), Times.Once);
    }

    [Test]
    public async Task InvoiceFailedHandleAsync_IfExistingSubscriptionIsNotEnds_ShouldLeaveCurrent()
    {
        // Arrange
        var userId = _fixture.Create<Guid>();
        var invoice = new Invoice()
        {
            SubscriptionId = _fixture.Create<string>(),
            Lines = new StripeList<InvoiceLineItem>()
            {
                Data = new List<InvoiceLineItem>()
                {
                    new InvoiceLineItem()
                    {
                        Price = new Price()
                        {
                            Id = _fixture.Create<string>()
                        }
                    }
                }
            },
            PeriodEnd = DateTime.UtcNow.AddDays(3),
            WebhooksDeliveredAt = DateTime.UtcNow
        };

        var freeProduct = new ProductModel()
        {
            Name = "Free",
            Price = 0,
            PriceId = _fixture.Create<string>()
        };

        var currentSubscription = new Subscription()
        {
            Items = new StripeList<SubscriptionItem>()
            {
                Data = new List<SubscriptionItem>()
                {
                    new SubscriptionItem()
                    {
                        Plan = new Plan()
                        {
                            ProductId = _fixture.Create<string>()
                        }
                    }
                }
            },
            CurrentPeriodEnd = DateTime.UtcNow.AddDays(3),
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.UserId, userId.ToString() }
            }
        };
        var newSubscription = new Subscription();

        _webhookService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);

        _webhookService.Protected().Setup<InvoiceService>(GET_INVOICE_SERVICE)
            .Returns(_invoiceService.Object);

        _subscriptionService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(currentSubscription);

        _productCatalogService.Setup(x => x.GetProductByNameAsync(It.Is<string>(a =>
            a.Equals(AccountPlan.Free.ToString())))).ReturnsAsync(freeProduct);

        _subscriptionService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(),
            null, default)).ReturnsAsync(newSubscription);

        _invoiceService.Setup(x => x.VoidInvoiceAsync(It.IsAny<string>(), null, null, default));

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionAsync(It.IsAny<DAL.Models.Subscription>(), null));

        _paymentRepository.Setup(x => x.CreatePaymentAsync(It.IsAny<Payment>()));

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
                       It.IsAny<SubscriptionStatusEnum>()));

        // Act
        await _webhookService.Object.InvoiceFailedHandleAsync(invoice);

        //Assert
        _subscriptionService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _productCatalogService.Verify(x => x.GetProductByNameAsync(It.Is<string>(a =>
            a.Equals(AccountPlan.Free.ToString()))), Times.Never);
        _subscriptionService.Verify(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(),
            null, default), Times.Once);
        _invoiceService.Verify(x => x.VoidInvoiceAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _paymentRepository.Verify(x => x.CreatePaymentAsync(It.IsAny<Payment>()), Times.Once);
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
                       It.IsAny<SubscriptionStatusEnum>()), Times.Once);
    }

    [Test]
    public async Task InvoiceSucceededHandleAsync_ShouldUpdateSubscriptionPlan()
    {
        var userId = _fixture.Create<Guid>();
        var invoice = new Invoice()
        {
            SubscriptionId = _fixture.Create<string>()
        };

        var currentSubscription = new Subscription()
        {
            Items = new StripeList<SubscriptionItem>()
            {
                Data = new List<SubscriptionItem>()
                {
                    new SubscriptionItem()
                    {
                        Plan = new Plan()
                        {
                            ProductId = _fixture.Create<string>()
                        }
                    }
                }
            },
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.UserId, userId.ToString() }
            }
        };
        var newSubscription = new Subscription();

        var productModel = new ProductModel()
        {
            Name = "Starter/someTranslate",
            Price = 10
        };

        _productCatalogService.Setup(x => x.GetProductAsync(It.IsAny<string>()))
            .ReturnsAsync(productModel);

        _webhookService.Protected().Setup<SubscriptionService>(GET_SUBSCRIPTION_SERVICE)
            .Returns(_subscriptionService.Object);

        _subscriptionService.Setup(x => x.GetAsync(It.IsAny<string>(), null, null, default))
            .ReturnsAsync(currentSubscription);

        _subscriptionService.Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<SubscriptionUpdateOptions>(),
                       null, default)).ReturnsAsync(newSubscription);

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionAsync(It.IsAny<DAL.Models.Subscription>(), null));

        _subscriptionRepository.Setup(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
                                  It.IsAny<SubscriptionStatusEnum>()));

        // Act
        await _webhookService.Object.InvoiceSucceededHandleAsync(invoice);

        //Assert
        _subscriptionService.Verify(x => x.GetAsync(It.IsAny<string>(), null, null, default), Times.Once);
        _subscriptionRepository.Verify(x => x.UpdateSubscriptionStatusAsync(It.IsAny<string>(),
                                  It.IsAny<SubscriptionStatusEnum>()), Times.Once);
    }
}
