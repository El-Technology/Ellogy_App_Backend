using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using PaymentManager.Common.Constants;
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

        public async Task CreatePaymentAsync(SessionCreateOptions options)
        {
            var busSender = _busClient.CreateSender(Constants.PaymentRequestQueueName);
            var message = new ServiceBusMessage(JsonConvert.SerializeObject(options))
            {
                ContentType = Constants.ApplicationJson
            };
            await busSender.SendMessageAsync(message);
        }
    }
}