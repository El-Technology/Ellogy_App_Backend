using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PaymentManager.DAL.Models;
using PaymentManager.DAL.Repositories;
using Stripe.Checkout;

namespace PaymentManager.BLL
{
    public class PaymentConsumer : IHostedService
    {
        private readonly ServiceBusClient _busClient;
        private readonly IServiceProvider _serviceProvider;

        public PaymentConsumer(ServiceBusClient busClient, IServiceProvider serviceProvider)
        {
            _busClient = busClient;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var service = new SessionService();


            var processorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                PrefetchCount = 1
            };

            var processor = _busClient.CreateProcessor("paymentrequestqueue", processorOptions);

            processor.ProcessMessageAsync += async (messageArg) =>
            {
                var message = JsonConvert.DeserializeObject<SessionCreateOptions>(messageArg.Message.Body.ToString());
                if (!IsValidEmail(message.CustomerEmail))
                {
                    await messageArg.DeadLetterMessageAsync(messageArg.Message);
                    throw new Exception($"Wrong email = {message.CustomerEmail}");
                }

                Session session = service.Create(message);

                await CreatePaymentAsync(new Payment
                {
                    Id = Guid.NewGuid(),
                    PaymentId = session.PaymentIntentId,
                    ProductId = Guid.Parse(session.Metadata["productId"]),
                    Status = "created",
                    UserEmail = session.CustomerEmail,
                    SessionId = session.Id,
                    UpdatedBallance = false,
                    UserId = Guid.Parse(session.Metadata["userId"])
                });

                Console.WriteLine(session.Url + $"\n{session.Id}"); //signalR

                await messageArg.CompleteMessageAsync(messageArg.Message);
            };

            processor.ProcessErrorAsync += async (messageArgs) =>
            {
                Console.WriteLine(messageArgs.Exception.Message);
            };

            await processor.StartProcessingAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        private async Task CreatePaymentAsync(Payment payment)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var testRepo = scope.ServiceProvider.GetRequiredService<TestRepo>();

            try
            {
                await testRepo.CreatePaymentAsync(payment);
            }
            finally
            {
                await scope.DisposeAsync();
            }
        }
    }
}
