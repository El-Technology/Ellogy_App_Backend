using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL;
using PaymentManager.BLL.Models;
using PaymentManager.BLL.Services;
using PaymentManager.DAL.Repositories;

namespace PaymentManager.Controllers
{
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

        [HttpGet]
        [Route("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _testRepo.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("OrderConfirmation")]
        public async Task<IActionResult> OrderConfirmation(string id)
        {
            var response = await _paymentService.OrderConfirmationAsync(id);
            return Ok(response);
        }

        [HttpPost]
        [Route("CreatePayment")]
        public async Task<IActionResult> CreatePayment(CreatePaymentDto createPaymentDto)
        {
            var payment = await _paymentService.CreatePaymentAsync(createPaymentDto);
            await _serviceBus.CreatePaymentAsync(payment, createPaymentDto.ProductId);
            return Ok();
        }

    }
}
