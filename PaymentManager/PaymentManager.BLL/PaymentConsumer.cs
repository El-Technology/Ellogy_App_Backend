using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace PaymentManager.BLL
{
    public class PaymentConsumer : IHostedService
    {
        private readonly ServiceBusClient _busClient = new(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING"));

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
                Session session = service.Create(JsonConvert.DeserializeObject<SessionCreateOptions>(messageArg.Message.Body.ToString()));
                Console.WriteLine(session.Url);
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
    }
}
