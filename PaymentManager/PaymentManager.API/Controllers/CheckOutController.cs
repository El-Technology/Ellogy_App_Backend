using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Services;
using PaymentManager.Common.Options;
using PaymentManager.DAL.Repositories;

namespace PaymentManager.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : Controller
    {
        private readonly PaymentProducer _serviceBus;
        private readonly PaymentService _paymentService;
        private readonly TestRepo _testRepo;

        public CheckOutController(PaymentProducer serviceBus, PaymentService paymentService, TestRepo testRepo)
        {
            _serviceBus = serviceBus;
            _paymentService = paymentService;
            _testRepo = testRepo;
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
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _testRepo.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _testRepo.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("OrderConfirmation")]
        public async Task<IActionResult> OrderConfirmation(string sessionId)
        {
            var response = await _paymentService.OrderConfirmationAsync(sessionId, GetUserIdFromToken());
            return Ok(response);
        }

        [HttpPost]
        [Route("CreatePayment")]
        public async Task<IActionResult> CreatePayment(Guid productId)
        {
            var payment = await _paymentService.CreatePaymentAsync(productId, GetUserIdFromToken());
            await _serviceBus.CreatePaymentAsync(payment, productId);
            return Ok();
        }

    }
}
