using AutoFixture;
using Moq;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.Tests.ServiceTests;

public class StripeBaseServiceForTests
{
    protected const string GET_INVOICE_SERVICE = "GetInvoiceService";
    protected const string GET_PRICE_SERVICE = "GetPriceService";
    protected const string GET_PAYMENT_INTENT_SERVICE = "GetPaymentIntentService";
    protected const string GET_CUSTOMER_SERVICE = "GetCustomerService";
    protected const string GET_PAYMENT_METHOD_SERVICE = "GetPaymentMethodService";
    protected const string GET_SUBSCRIPTION_SERVICE = "GetSubscriptionService";
    protected const string GET_PRODUCT_SERVICE = "GetProductService";
    protected const string GET_SESSION_SERVICE = "GetSessionService";

    protected Mock<CustomerService> _customerService;
    protected Fixture _fixture = new();
    protected Mock<InvoiceService> _invoiceService;
    protected Mock<PaymentIntentService> _paymentIntentService;
    protected Mock<PaymentMethodService> _paymentMethodService;
    protected Mock<PriceService> _priceService;
    protected Mock<ProductService> _productService;
    protected Mock<SessionService> _sessionService;
    protected Mock<SubscriptionService> _subscriptionService;
}