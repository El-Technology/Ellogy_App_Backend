using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Interfaces;
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
        private readonly ProductCatalogService _productCatalogService;

        public CheckOutController(PaymentProducer serviceBus,
            IPaymentSessionService paymentService,
            PaymentSubscriptionService paymentSubscriptionService,
            PaymentCustomerService paymentCustomerService,
            ProductCatalogService productCatalogService)
        {
            _serviceBus = serviceBus;
            _paymentService = paymentService;
            _paymentSubscriptionService = paymentSubscriptionService;
            _paymentCustomerService = paymentCustomerService;
            _productCatalogService = productCatalogService;
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

        // subscription separate logic below

        [HttpGet]
        [Route("createCustomer")]
        public async Task<IActionResult> CreateCustomer()
        {
            await _paymentCustomerService.CreateCustomerAsync(GetUserIdFromToken());
            return Ok();
        }

        [HttpGet]
        [Route("updateCustomedData")]
        public async Task<IActionResult> UpdateCustomerData()
        {
            await _paymentCustomerService.UpdateCustomerDataAsync(GetUserIdFromToken());
            return Ok();
        }

        [HttpGet]
        [Route("updateCustomedPaymentMethod")]
        public async Task<IActionResult> UpdateCustomerPaymentMethod()
        {
            return Ok(await _paymentCustomerService.UpdateCustomerPaymentMethodAsync(GetUserIdFromToken()));
        }

        [HttpGet]
        [Route("createSubscription")]
        public async Task<IActionResult> CreateSubscription([FromQuery] string priceId)
        {
            return Ok(await _paymentSubscriptionService.CreateSubscriptionAsync(GetUserIdFromToken(), priceId));
        }

        [HttpGet]
        [Route("cancelSubscription")]
        public async Task<IActionResult> CancelSubscription([FromQuery] bool cancelNow, [FromQuery] string paymentIntent)
        {
            await _paymentSubscriptionService.CancelSubscriptionAsync(GetUserIdFromToken(), cancelNow, paymentIntent);
            return Ok();
        }

        [HttpGet]
        [Route("getProducts")]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(_productCatalogService.GetSubscriptionCatalogAsync());
        }
    }
}
