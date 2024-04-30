using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : Controller
{
    private readonly IWebhookService _webhookService;

    public WebhookController(IWebhookService webhookService)
    {
        _webhookService = webhookService;
    }

    /// <summary>
    ///     This method handles stripe events
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], await EnvironmentVariables.WebhookKey);

            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:
                    var completedSession = (Session)stripeEvent.Data.Object;
                    await _webhookService.OrderConfirmationAsync(completedSession);
                    break;
                case Events.CheckoutSessionExpired:
                    var expiredSession = (Session)stripeEvent.Data.Object;
                    await _webhookService.ExpireSessionAsync(expiredSession);
                    break;
                case Events.CustomerCreated:
                    var customerCreated = (Customer)stripeEvent.Data.Object;
                    await _webhookService.CreateCustomerAsync(customerCreated);
                    break;
                case Events.CustomerSubscriptionUpdated:
                    var subscriptionUpdated = (Subscription)stripeEvent.Data.Object;
                    await _webhookService.UpdateSubscriptionAsync(subscriptionUpdated);
                    break;
                case Events.CustomerSubscriptionDeleted:
                    var subscriptionDeleted = (Subscription)stripeEvent.Data.Object;
                    await _webhookService.DeleteSubscriptionAsync(subscriptionDeleted);
                    break;
                case Events.InvoicePaymentSucceeded:
                    var invoiceSucceeded = (Invoice)stripeEvent.Data.Object;
                    await _webhookService.InvoiceSucceededHandleAsync(invoiceSucceeded);
                    break;
                case Events.InvoicePaymentFailed:
                    var invoiceFailed = (Invoice)stripeEvent.Data.Object;
                    await _webhookService.InvoiceFailedHandleAsync(invoiceFailed);
                    break;
                default:
                    throw new Exception("Unknown error");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}