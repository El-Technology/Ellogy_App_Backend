using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Options;

namespace PaymentManager.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly IPaymentCustomerService _paymentCustomerService;
        private readonly PaymentProducer _serviceBus;
        public CustomerController(PaymentProducer serviceBus, IPaymentCustomerService paymentCustomerService)
        {
            _paymentCustomerService = paymentCustomerService;
            _serviceBus = serviceBus;
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
        [Route("createCustomer")]
        public async Task<IActionResult> CreateCustomer()
        {
            await _paymentCustomerService.CreateCustomerAsync(GetUserIdFromToken());
            return Ok();
        }

        [HttpGet]
        [Route("updateCustomerData")]
        public async Task<IActionResult> UpdateCustomerData()
        {
            await _paymentCustomerService.UpdateCustomerDataAsync(GetUserIdFromToken());
            return Ok();
        }

        [HttpPost]
        [Route("addCustomerPaymentMethod")]
        public async Task<IActionResult> AddCustomerPaymentMethod([FromBody] CreateSessionRequest createSessionRequest)
        {
            var sessionCreateOptions = await _paymentCustomerService.AddCustomerPaymentMethodAsync(GetUserIdFromToken(), createSessionRequest);
            await _serviceBus.CreateSessionAsync(sessionCreateOptions);
            return Ok();
        }

        [HttpGet]
        [Route("getAllPaymentMethods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            return Ok(await _paymentCustomerService.RetrieveCustomerPaymentMethodsAsync(GetUserIdFromToken()));
        }

        [HttpGet]
        [Route("setDefaultPaymentMethod")]
        public async Task<IActionResult> SetDefaultPaymentMethod([FromQuery] string paymentMethodId)
        {
            await _paymentCustomerService.SetDefaultPaymentMethodAsync(GetUserIdFromToken(), paymentMethodId);
            return Ok();
        }

        [HttpGet]
        [Route("getCustomerPayments")]
        public async Task<IActionResult> GetCustomerPayments()
        {
            return Ok(await _paymentCustomerService.GetCustomerPaymentsAsync(GetUserIdFromToken()));
        }

        [HttpGet]
        [Route("getActiveSubscription")]
        public async Task<IActionResult> GetActiveSubscription()
        {
            return Ok(await _paymentCustomerService.GetActiveSubscriptionAsync(GetUserIdFromToken()));
        }

        [HttpGet]
        [Route("updateSubscriptionPreview")]
        public async Task<IActionResult> UpdateSubscriptionPreview(string newPriceId)
        {
            return Ok(await _paymentCustomerService.UpgradeSubscriptionPreviewAsync(GetUserIdFromToken(), newPriceId));
        }

        [HttpGet]
        [Route("detachPaymentMethod")]
        public async Task<IActionResult> DetachPaymentMethod(string paymentMethodId)
        {
            await _paymentCustomerService.DetachPaymentMethodAsync(paymentMethodId);
            return Ok();
        }
    }
}
