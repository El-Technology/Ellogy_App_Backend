using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using PaymentManager.Common.Dtos;
using PaymentManager.DAL.Interfaces;
using Stripe.Checkout;

namespace PaymentManager.BLL.Services
{
    public class PaymentCustomerService : StripeBaseService, IPaymentCustomerService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public PaymentCustomerService(IUserRepository userRepository,
            IPaymentRepository paymentRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _paymentRepository = paymentRepository;
            _userRepository = userRepository;
        }

        /// <inheritdoc cref="IPaymentCustomerService.CreateCustomerAsync(Guid)"/>
        public async Task CreateCustomerAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (!string.IsNullOrEmpty(user.StripeCustomerId))
                throw new Exception("Customer is already created");

            var customerData = await GetCustomerService().CreateAsync(new()
            {
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}",
                Metadata = new() { { MetadataConstants.UserId, userId.ToString() } }
            });

            await _userRepository.AddStripeCustomerIdAsync(userId, customerData.Id);
        }

        /// <inheritdoc cref="IPaymentCustomerService.UpdateCustomerDataAsync(Guid)"/>
        public async Task UpdateCustomerDataAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (string.IsNullOrEmpty(user.StripeCustomerId))
                throw new Exception("Customer is not created");

            await GetCustomerService().UpdateAsync(user.StripeCustomerId, new()
            {
                Email = user.Email,
                Name = $"{user.FirstName} {user.LastName}"
            });
        }

        /// <inheritdoc cref="IPaymentCustomerService.AddCustomerPaymentMethodAsync(Guid, CreateSessionRequest)"/>
        public async Task<SessionCreateOptions> AddCustomerPaymentMethodAsync(Guid userId, CreateSessionRequest createSessionRequest)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (user.StripeCustomerId is null)
                throw new ArgumentNullException("Customer is not created");

            var sessionOptions = new SessionCreateOptions
            {
                SuccessUrl = createSessionRequest.SuccessUrl,
                CancelUrl = createSessionRequest.CancelUrl,
                Mode = Constants.SETUP_MODE,
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

        /// <inheritdoc cref="IPaymentCustomerService.RetrieveCustomerPaymentMethodsAsync(Guid, StripePaginationRequestDto)"/>
        public async Task<IEnumerable<PaymentMethod>> RetrieveCustomerPaymentMethodsAsync(Guid userId, StripePaginationRequestDto paginationRequestDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            if (string.IsNullOrEmpty(user.StripeCustomerId))
                throw new Exception("Customer is not created");

            var allMethods = await GetPaymentMethodService().ListAsync(new()
            {
                Customer = user.StripeCustomerId,
                Expand = new() { "data.customer.invoice_settings.default_payment_method" },
                Limit = paginationRequestDto.RecordsPerPage,
                StartingAfter = paginationRequestDto.StartAfter,
                EndingBefore = paginationRequestDto.EndBefore
            });

            var paymentMethods = new List<PaymentMethod>();

            foreach (var method in allMethods)
            {
                if (method.Card is not null)
                {
                    var paymentMethod = new PaymentMethod
                    {
                        Type = method.Type,
                        Id = method.Id,
                        CardBrand = method.Card.Brand,
                        Expires = $"{method.Card.ExpMonth}/{method.Card.ExpYear}",
                        Last4 = method.Card.Last4,
                        Default = (method.Customer.InvoiceSettings.DefaultPaymentMethodId ?? string.Empty).Equals(method.Id)
                    };
                    paymentMethods.Add(paymentMethod);
                }
                else
                {
                    var paymentMethod = new PaymentMethod
                    {
                        Type = method.Type,
                        Id = method.Id,
                        Default = (method.Customer.InvoiceSettings.DefaultPaymentMethodId ?? string.Empty).Equals(method.Id)
                    };
                    paymentMethods.Add(paymentMethod);
                }
            }

            return paymentMethods;
        }

        /// <inheritdoc cref="IPaymentCustomerService.SetDefaultPaymentMethodAsync(Guid, string)"/>
        public async Task SetDefaultPaymentMethodAsync(Guid userId, string paymentMethodId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            await GetCustomerService().UpdateAsync(user.StripeCustomerId, new()
            {
                InvoiceSettings = new()
                {
                    DefaultPaymentMethod = paymentMethodId,
                }
            });

            var customer = await GetCustomerService().GetAsync(user.StripeCustomerId, new()
            {
                Expand = new() { "subscriptions" }
            });

            var customerSubscription = customer.Subscriptions.FirstOrDefault();

            if (customerSubscription is null)
                return;

            await GetSubscriptionService().UpdateAsync(customerSubscription.Id, new()
            {
                DefaultPaymentMethod = paymentMethodId
            });
        }

        /// <inheritdoc cref="IPaymentCustomerService.GetCustomerPaymentsAsync(Guid, StripePaginationRequestDto)"/>
        public async Task<IEnumerable<PaymentObject>> GetCustomerPaymentsAsync(Guid userId, StripePaginationRequestDto paginationRequestDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new ArgumentNullException(nameof(userId));

            var paymentsList = await GetPaymentIntentService().ListAsync(new()
            {
                Customer = user.StripeCustomerId,
                Expand = new() { "data.invoice" },
                Limit = paginationRequestDto.RecordsPerPage,
                EndingBefore = paginationRequestDto.EndBefore,
                StartingAfter = paginationRequestDto.StartAfter
            });

            var paymentRecords = new List<PaymentObject>();

            foreach (var payment in paymentsList.Data)
            {
                var paymentRecord = await _paymentRepository.GetPaymentByIdAsync(payment.Id)
                    ?? await _paymentRepository.GetPaymentByInvoiceIdAsync(payment.InvoiceId);

                var paymentObject = new PaymentObject
                {
                    Product = paymentRecord is null ? "Payment" : paymentRecord.ProductName,
                    Date = payment.Created,
                    Amount = payment.Amount / Constants.PriceInCents,
                    Status = payment.Status,
                    DownloadLink = payment.Invoice?.InvoicePdf
                };

                paymentRecords.Add(paymentObject);
            }

            return paymentRecords;
        }


        /// <inheritdoc cref="IPaymentCustomerService.GetActiveSubscriptionAsync(Guid)"/>
        public async Task<DAL.Models.Subscription?> GetActiveSubscriptionAsync(Guid userId)
        {
            return await _subscriptionRepository.GetActiveSubscriptionAsync(userId);
        }

        public async Task<decimal> UpgradeSubscriptionPreviewAsync(Guid userId, string newPriceId)
        {
            var getActiveSubscription = await GetActiveSubscriptionAsync(userId)
                ?? throw new Exception("You don`t have active subscription");

            var newPriceInformation = await GetPriceService().GetAsync(newPriceId);

            if (newPriceInformation.UnitAmountDecimal / Constants.PriceInCents <= getActiveSubscription.Price)
                throw new Exception("You can`t upgrade on a subscription with the same or a lower price");

            var user = await _userRepository.GetUserByIdAsync(userId)
                ?? throw new Exception("User was not found");

            var subscription = await GetSubscriptionService().GetAsync(getActiveSubscription.SubscriptionStripeId);

            if (subscription.Items.Data.Count > 1)
                throw new Exception($"You cant upgrade your subscription not, wait until {subscription.CurrentPeriodEnd}");

            var result = await GetInvoiceService().UpcomingAsync(new()
            {
                Customer = user.StripeCustomerId,
                Subscription = getActiveSubscription.SubscriptionStripeId,
                SubscriptionItems = new()
                {
                    new(){ Id = subscription.Items.Data.First().Id, Deleted = true },
                    new(){ Price = newPriceId }
                },
                SubscriptionProrationDate = DateTime.UtcNow,
                SubscriptionProrationBehavior = "always_invoice"
            });

            return result.AmountDue / Constants.PriceInCents;
        }

        public async Task DetachPaymentMethodAsync(string paymentMethodId)
        {
            await GetPaymentMethodService().DetachAsync(paymentMethodId);
        }
    }
}
