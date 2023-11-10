using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Stripe.Checkout;

namespace PaymentManager.BLL
{
    public class PaymentProducer
    {
        private readonly ServiceBusClient _busClient;

        public PaymentProducer(ServiceBusClient busClient)
        {
            _busClient = busClient;
        }

        public async Task CreatePaymentAsync(SessionCreateOptions options, Guid productId)
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