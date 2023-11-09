using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace PaymentManager.BLL
{
    public class PaymentProducer
    {
        private readonly ServiceBusClient _busClient = new(Environment.GetEnvironmentVariable("AZURE_SERVICE_BUS_CONNECTION_STRING"));

        public async Task CreatePaymentAsync(SessionCreateOptions options)
        {
            var busSender = _busClient.CreateSender("paymentrequestqueue");

            var message = new ServiceBusMessage(JsonConvert.SerializeObject(options))
            {
                ContentType = "application/json"
            };

            await busSender.SendMessageAsync(message);
        }
    }
}