using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.Models;
using Stripe.Checkout;

namespace PaymentManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : Controller
    {
        private string SessionId;
        private readonly PaymentProducer _serviceBus;

        public CheckOutController(PaymentProducer serviceBus)
        {
            _serviceBus = serviceBus;
        }

        [HttpGet]
        [Route("index")]
        public IActionResult Index()
        {
            var listOfProducts = new List<ProductModel>()
            {
                new ProductModel
                {
                    Price = 100,
                    Title = "100 points"
                }
            };
            return Ok(listOfProducts);
        }

        //[HttpGet]
        //[Route("OrderConfirmation")]
        //public IActionResult OrderConfirmation()
        //{
        //    var serivce = new SessionService();
        //    Session session = serivce.Get(SessionId);
        //    if (session.PaymentStatus == "Paid")
        //    {
        //        return Ok("Success");
        //    }
        //    return Ok("Login");
        //}

        //[HttpGet]
        //[Route("checkOut")]
        //public IActionResult CheckOut()
        //{




        //    SessionId = session.Id;
        //    return Ok(session);
        //}

        [HttpPost]
        [Route("CreatePayment")]
        public async Task<IActionResult> CreatePayment()
        {
            var listOfProducts = new List<ProductModel>()
            {
                new ProductModel
                {
                    Price = 1,
                    Title = "1000 points"
                }
            };

            var options = new SessionCreateOptions()
            {
                SuccessUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation",
                CancelUrl = $"https://localhost:7267/api/CheckOut/OrderConfirmation",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = "sharkovskiy1@gmail.com"
            };

            foreach (var item in listOfProducts)
            {
                var sessionListItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Title,
                        },
                        UnitAmount = item.Price * 100,
                    },
                    Quantity = 1
                };
                options.LineItems.Add(sessionListItem);
            }



            await _serviceBus.CreatePaymentAsync(options);
            return Ok();
        }

    }
}
