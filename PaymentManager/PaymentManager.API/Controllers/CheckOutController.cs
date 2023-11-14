using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Options;

namespace PaymentManager.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : Controller
    {
        private readonly PaymentProducer _serviceBus;
        private readonly IPaymentService _paymentService;

        public CheckOutController(PaymentProducer serviceBus, IPaymentService paymentService)
        {
            _serviceBus = serviceBus;
            _paymentService = paymentService;
        }

        /// <summary>
        /// This method retrieves the user id from the JWT token
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private Guid GetUserIdFromToken()
        {
            var status = Guid.TryParse(User.FindFirst(JwtOptions.UserIdClaimName)?.Value, out Guid userId);
            if (!status)
                throw new Exception("Taking user id error, try again later");

            return userId;
        }

        [HttpGet]
        [Route("getUserBalance")]
        public async Task<IActionResult> GetUserBalance()
        {
            var products = await _paymentService.GetUserBalanceAsync(GetUserIdFromToken());
            return Ok(products);
        }

        [HttpPost]
        [Route("createPayment")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest streamRequest)
        {
            var payment = await _paymentService.CreatePaymentAsync(GetUserIdFromToken(), streamRequest);
            await _serviceBus.CreatePaymentAsync(payment);
            return Ok();
        }
    }
}
