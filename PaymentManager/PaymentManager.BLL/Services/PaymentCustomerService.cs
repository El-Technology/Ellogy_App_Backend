using AutoMapper;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Interfaces.IHttpServices;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.Common.Dtos;
using PaymentManager.DAL.Interfaces;
using Stripe;
using Stripe.Checkout;
using Subscription = PaymentManager.DAL.Models.Subscription;

namespace PaymentManager.BLL.Services;

/// <summary>
///     Service for managing customer payments and subscriptions.
/// </summary>
public class PaymentCustomerService : StripeBaseService, IPaymentCustomerService
{
    private readonly IMapper _mapper;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserExternalHttpService _userExternalHttpService;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentCustomerService(ISubscriptionRepository subscriptionRepository,
        IMapper mapper,
        IUserExternalHttpService userExternalHttpService,
        IPaymentRepository paymentRepository)
    {
        _userExternalHttpService = userExternalHttpService;
        _mapper = mapper;
        _subscriptionRepository = subscriptionRepository;
        _paymentRepository = paymentRepository;
    }

    /// <inheritdoc cref="IPaymentCustomerService.CreateCustomerAsync(Guid)" />
    public async Task CreateCustomerAsync(Guid userId)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        if (!string.IsNullOrEmpty(user.StripeCustomerId))
            throw new Exception("Customer is already created");

        await _paymentRepository.CreateUserWalletAsync(userId);

        var customerData = await GetCustomerService().CreateAsync(new CustomerCreateOptions
        {
            Email = user.Email,
            Name = $"{user.FirstName} {user.LastName}",
            Metadata = new Dictionary<string, string> { { MetadataConstants.UserId, userId.ToString() } }
        });

        await _userExternalHttpService.AddStripeCustomerIdAsync(userId, customerData.Id);
    }

    /// <inheritdoc cref="IPaymentCustomerService.UpdateCustomerDataAsync(Guid)" />
    public async Task UpdateCustomerDataAsync(Guid userId)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        ArgumentNullException.ThrowIfNull(user.StripeCustomerId, nameof(user.StripeCustomerId));

        await GetCustomerService().UpdateAsync(user.StripeCustomerId, new CustomerUpdateOptions
        {
            Email = user.Email,
            Name = $"{user.FirstName} {user.LastName}"
        });
    }

    /// <inheritdoc cref="IPaymentCustomerService.AddCustomerPaymentMethodAsync(Guid, CreateSessionRequest)" />
    public async Task<SessionCreateOptions> AddCustomerPaymentMethodAsync(Guid userId,
        CreateSessionRequest createSessionRequest)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        ArgumentNullException.ThrowIfNull(user.StripeCustomerId, nameof(user.StripeCustomerId));

        var sessionOptions = new SessionCreateOptions
        {
            SuccessUrl = createSessionRequest.SuccessUrl,
            CancelUrl = createSessionRequest.CancelUrl,
            Mode = Constants.SetupMode,
            Currency = Constants.ApplicationCurrency,
            Customer = user.StripeCustomerId,
            Metadata = new Dictionary<string, string>
            {
                { MetadataConstants.UserId, user.Id.ToString() },
                { MetadataConstants.ConnectionId, createSessionRequest.ConnectionId },
                { MetadataConstants.SignalRMethodName, createSessionRequest.SignalMethodName }
            }
        };

        return sessionOptions;
    }

    /// <inheritdoc cref="IPaymentCustomerService.RetrieveCustomerPaymentMethodsAsync(Guid, StripePaginationRequestDto)" />
    public async Task<StripePaginationResponseDto<IEnumerable<PaymentMethodDto>>> RetrieveCustomerPaymentMethodsAsync(
        Guid userId, StripePaginationRequestDto paginationRequestDto)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        ArgumentNullException.ThrowIfNull(user.StripeCustomerId, nameof(user.StripeCustomerId));

        var allMethods = await GetPaymentMethodService().ListAsync(new PaymentMethodListOptions
        {
            Customer = user.StripeCustomerId,
            Expand = new List<string> { "data.customer.invoice_settings.default_payment_method" },
            Limit = paginationRequestDto.RecordsPerPage,
            StartingAfter = paginationRequestDto.StartAfter,
            EndingBefore = paginationRequestDto.EndBefore
        });

        return new StripePaginationResponseDto<IEnumerable<PaymentMethodDto>>
        {
            HasMore = allMethods.HasMore,
            Data = _mapper.Map<List<PaymentMethodDto>>(allMethods)
        };
    }

    /// <inheritdoc cref="IPaymentCustomerService.SetDefaultPaymentMethodAsync(Guid, string)" />
    public async Task SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        ArgumentNullException.ThrowIfNull(user.StripeCustomerId, nameof(user.StripeCustomerId));

        await GetCustomerService().UpdateAsync(user.StripeCustomerId, new CustomerUpdateOptions
        {
            InvoiceSettings = new CustomerInvoiceSettingsOptions
            {
                DefaultPaymentMethod = paymentMethodId
            }
        });

        var customer = await GetCustomerService().GetAsync(user.StripeCustomerId, new CustomerGetOptions
        {
            Expand = new List<string> { "subscriptions" }
        });

        var customerSubscription = customer.Subscriptions.FirstOrDefault();

        if (customerSubscription is null)
            return;

        await GetSubscriptionService().UpdateAsync(customerSubscription.Id, new SubscriptionUpdateOptions
        {
            DefaultPaymentMethod = paymentMethodId
        });
    }

    /// <inheritdoc cref="IPaymentCustomerService.GetCustomerPaymentsAsync(Guid, StripePaginationRequestDto)" />
    public async Task<StripePaginationResponseDto<IEnumerable<PaymentObject>>> GetCustomerPaymentsAsync(Guid userId,
        StripePaginationRequestDto paginationRequestDto)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        ArgumentNullException.ThrowIfNull(user.StripeCustomerId, nameof(user.StripeCustomerId));

        var paymentsList = await GetPaymentIntentService().ListAsync(new PaymentIntentListOptions
        {
            Customer = user.StripeCustomerId,
            Expand = new List<string> { "data.invoice" },
            Limit = paginationRequestDto.RecordsPerPage,
            EndingBefore = paginationRequestDto.EndBefore,
            StartingAfter = paginationRequestDto.StartAfter
        });

        return new StripePaginationResponseDto<IEnumerable<PaymentObject>>
        { HasMore = paymentsList.HasMore, Data = _mapper.Map<List<PaymentObject>>(paymentsList) };
    }


    /// <inheritdoc cref="IPaymentCustomerService.GetActiveSubscriptionAsync(Guid)" />
    public async Task<Subscription?> GetActiveSubscriptionAsync(Guid userId)
    {
        return await _subscriptionRepository.GetActiveSubscriptionAsync(userId);
    }

    /// <inheritdoc cref="IPaymentCustomerService.UpgradeSubscriptionPreviewAsync(Guid, string)" />
    public async Task<decimal> UpgradeSubscriptionPreviewAsync(Guid userId, string newPriceId)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
           ?? throw new Exception("User was not found");

        var getActiveSubscription = await GetActiveSubscriptionAsync(userId)
                                    ?? throw new Exception("You don`t have active subscription");

        var newPriceInformation = await GetPriceService().GetAsync(newPriceId);

        if (newPriceInformation.UnitAmountDecimal / Constants.PriceInCents <= getActiveSubscription.Price)
            throw new Exception("You can`t upgrade on a subscription with the same or a lower price");

        var subscription = await GetSubscriptionService().GetAsync(getActiveSubscription.SubscriptionStripeId);

        if (subscription.Items.Data.Count > 1)
            throw new Exception($"You cant upgrade your subscription not, wait until {subscription.CurrentPeriodEnd}");

        var result = await GetInvoiceService().UpcomingAsync(new UpcomingInvoiceOptions
        {
            Customer = user.StripeCustomerId,
            Subscription = getActiveSubscription.SubscriptionStripeId,
            SubscriptionItems = new List<InvoiceSubscriptionItemOptions>
            {
                new() { Id = subscription.Items.Data.First().Id, Deleted = true },
                new() { Price = newPriceId }
            },
            SubscriptionProrationDate = DateTime.UtcNow,
            SubscriptionProrationBehavior = "always_invoice"
        });

        return result.AmountDue / Constants.PriceInCents;
    }

    /// <inheritdoc cref="IPaymentCustomerService.DetachPaymentMethodAsync(string)" />
    public async Task DetachPaymentMethodAsync(string paymentMethodId)
    {
        await GetPaymentMethodService().DetachAsync(paymentMethodId);
    }

    /// <inheritdoc cref="IPaymentCustomerService.DeleteCustomerAsync(Guid)" />
    public async Task DeleteCustomerAsync(Guid userId)
    {
        var user = await _userExternalHttpService.GetUserByIdAsync(userId)
                   ?? throw new ArgumentNullException(nameof(userId));

        await GetCustomerService().DeleteAsync(user.StripeCustomerId);
        await _userExternalHttpService.RemoveStripeCustomerIdAsync(userId);
    }
}