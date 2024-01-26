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
        private readonly IPaymentSessionService _paymentService;

        public WebhookController(IPaymentSessionService paymentService)
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
                      //Request.Headers["Stripe-Signature"], EnvironmentVariables.WebhookKey);
                      Request.Headers["Stripe-Signature"], "whsec_5b9a006da20c4f3853b0364e4771ece0c1caa3ed956f89b1828ceb174b8f0274");

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
