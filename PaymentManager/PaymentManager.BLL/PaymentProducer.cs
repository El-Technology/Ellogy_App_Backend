using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.BLL;

public class PaymentProducer
{
    private readonly ServiceBusClient _busClient;

    public PaymentProducer(ServiceBusClient busClient)
    {
        _busClient = busClient;
    }

    public async Task CreateSessionAsync(SessionCreateOptions options)
    {
        await SendMessageAsync(options, Constants.PaymentMode);
    }

    public async Task CreateFreeSubscriptionAsync(SubscriptionCreateOptions options)
    {
        await SendMessageAsync(options, Constants.SubscriptionMode);
    }

    private async Task SendMessageAsync<T>(T options, string type) where T : class
    {
        var messageQueueModel = new MessageQueueModel<T>
        {
            Type = type,
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