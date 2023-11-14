using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using PaymentManager.Common.Constants;
using Stripe.Checkout;
using System.Text;

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
            var busSender = _busClient.CreateSender(Constants.PaymentQueueName);
            var messageBody = JsonConvert.SerializeObject(options);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
            {
                ContentType = Constants.ApplicationJson
            };
            await busSender.SendMessageAsync(message);
        }
    }
}