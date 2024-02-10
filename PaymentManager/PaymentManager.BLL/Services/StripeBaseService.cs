using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services;

/// <summary>
///     Base service for retrieving Stripe services.
/// </summary>
public class StripeBaseService
{
    private Lazy<CustomerService>? _customerService;
    private Lazy<InvoiceService>? _invoiceService;
    private Lazy<PaymentIntentService>? _paymentIntentService;
    private Lazy<PaymentMethodService>? _paymentMethodService;
    private Lazy<PriceService>? _priceService;
    private Lazy<ProductService>? _productService;
    private Lazy<SessionService>? _sessionService;
    private Lazy<SubscriptionService>? _subscriptionService;

    protected PriceService GetPriceService()
    {
        _priceService ??= new Lazy<PriceService>(() => new PriceService());
        return _priceService.Value;
    }

    protected InvoiceService GetInvoiceService()
    {
        _invoiceService ??= new Lazy<InvoiceService>(() => new InvoiceService());
        return _invoiceService.Value;
    }

    protected CustomerService GetCustomerService()
    {
        _customerService ??= new Lazy<CustomerService>(() => new CustomerService());
        return _customerService.Value;
    }

    protected PaymentMethodService GetPaymentMethodService()
    {
        _paymentMethodService ??= new Lazy<PaymentMethodService>(() => new PaymentMethodService());
        return _paymentMethodService.Value;
    }

    protected SubscriptionService GetSubscriptionService()
    {
        _subscriptionService ??= new Lazy<SubscriptionService>(() => new SubscriptionService());
        return _subscriptionService.Value;
    }

    protected PaymentIntentService GetPaymentIntentService()
    {
        _paymentIntentService ??= new Lazy<PaymentIntentService>(() => new PaymentIntentService());
        return _paymentIntentService.Value;
    }

    protected ProductService GetProductService()
    {
        _productService ??= new Lazy<ProductService>(() => new ProductService());
        return _productService.Value;
    }

    protected SessionService GetSessionService()
    {
        _sessionService ??= new Lazy<SessionService>(() => new SessionService());
        return _sessionService.Value;
    }
}