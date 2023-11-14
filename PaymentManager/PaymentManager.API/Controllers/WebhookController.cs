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
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Console.WriteLine(json);
            try
            {
                Console.WriteLine(Request.Headers["Stripe-Signature"]);
                Console.WriteLine(EnvironmentVariables.WebhookKey);

                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], EnvironmentVariables.WebhookKey);

                Console.WriteLine(stripeEvent.Type);

                switch (stripeEvent.Type)
                {
                    case Events.CheckoutSessionCompleted:
                        var session = (Session)stripeEvent.Data.Object;
                        await _paymentService.OrderConfirmationAsync(session.Id);
                        break;
                    default:
                        Console.WriteLine("error   "+stripeEvent.Type + $"\n{Events.CheckoutSessionCompleted}");
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
