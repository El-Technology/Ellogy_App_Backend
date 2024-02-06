using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using Stripe;
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

        public async Task CreateSessionAsync(SessionCreateOptions options)
        {
            var messageQueueModel = new MessageQueueModel<SessionCreateOptions>
            {
                Type = "session",
                CreateOptions = options
            };

            var busSender = _busClient.CreateSender(Constants.PaymentQueueName);
            var messageBody = JsonConvert.SerializeObject(messageQueueModel);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
            {
                ContentType = Constants.ApplicationJson
            };
            await busSender.SendMessageAsync(message);
        }

        public async Task CreateFreeSubscriptionAsync(SubscriptionCreateOptions options)
        {
            var messageQueueModel = new MessageQueueModel<SubscriptionCreateOptions>
            {
                Type = "subscription",
                CreateOptions = options
            };

            var busSender = _busClient.CreateSender(Constants.PaymentQueueName);
            var messageBody = JsonConvert.SerializeObject(messageQueueModel);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
            {
                ContentType = Constants.ApplicationJson
            };
            await busSender.SendMessageAsync(message);
        }
    }
}