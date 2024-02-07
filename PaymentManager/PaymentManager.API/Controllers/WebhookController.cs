using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL.Interfaces;
using PaymentManager.Common;
using Stripe;
using Stripe.Checkout;

namespace PaymentManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IWebhookService _webhookService;

        public WebhookController(IWebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                      Request.Headers["Stripe-Signature"], EnvironmentVariables.WebhookKey);

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
                        await _webhookService.PaymentSucceededHandleAsync(invoiceSucceeded);
                        break;
                    case Events.InvoicePaymentFailed:
                        var invoiceFailed = (Invoice)stripeEvent.Data.Object;
                        await _webhookService.PaymentFailedHandleAsync(invoiceFailed);
                        break;
                    default:
                        throw new Exception("Unknown error");
                }
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
