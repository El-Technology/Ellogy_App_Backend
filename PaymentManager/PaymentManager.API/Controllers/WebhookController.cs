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
        private readonly IPaymentService _paymentService;

        public WebhookController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
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
                        await _paymentService.OrderConfirmationAsync(completedSession.Id);
                        break;
                    case Events.CheckoutSessionExpired:
                        var expiredSession = (Session)stripeEvent.Data.Object;
                        await _paymentService.ExpireSessionAsync(expiredSession.Id);
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
