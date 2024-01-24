using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Options;

namespace PaymentManager.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : Controller
    {
        private readonly PaymentProducer _serviceBus;
        private readonly IPaymentSessionService _paymentService;
        private readonly PaymentCustomerService _paymentCustomerService;
        private readonly PaymentSubscriptionService _paymentSubscriptionService;

        public CheckOutController(PaymentProducer serviceBus,
            IPaymentSessionService paymentService,
            PaymentSubscriptionService paymentSubscriptionService,
            PaymentCustomerService paymentCustomerService)
        {
            _serviceBus = serviceBus;
            _paymentService = paymentService;
            _paymentSubscriptionService = paymentSubscriptionService;
            _paymentCustomerService = paymentCustomerService;
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

        [HttpGet]
        [Route("expireSession")]
        public async Task<IActionResult> ExpireSession(string sessionId)
        {
            await _paymentService.ExpireSessionAsync(sessionId);
            return Ok();
        }

        [HttpGet]
        [Route("createCustomer")]
        public async Task<IActionResult> CreateCustomer()
        {
            await _paymentCustomerService.CreateCustomerAsync(GetUserIdFromToken());
            return Ok();
        }

        [HttpGet]
        [Route("createSubscription")]
        public async Task<IActionResult> createSubscription()
        {
            return Ok(await _paymentSubscriptionService.CreateSubscriptionAsync(GetUserIdFromToken()));
        }

        [HttpGet]
        [Route("cancelSubscription")]
        public async Task<IActionResult> cancelSubscription()
        {
            await _paymentSubscriptionService.CancelSubscriptionAsync(GetUserIdFromToken());
            return Ok();
        }
    }
}
