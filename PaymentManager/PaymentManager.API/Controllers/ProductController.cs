using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL.Interfaces;

namespace PaymentManager.API.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductCatalogService _productCatalogService;
        public ProductController(IProductCatalogService productCatalogService)
        {
            _productCatalogService = productCatalogService;
        }

        [HttpGet]
        [Route("getProducts")]
        public IActionResult GetProducts()
        {
            return Ok(_productCatalogService.GetSubscriptionCatalogAsync());
        }
    }
}
