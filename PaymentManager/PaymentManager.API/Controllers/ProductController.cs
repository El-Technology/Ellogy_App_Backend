using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentManager.BLL.Interfaces;

namespace PaymentManager.API.Controllers;

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

    /// <summary>
    /// This method retrieves the list of products
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getProducts")]
    public async Task<IActionResult> GetProducts()
    {
        return Ok(await _productCatalogService.GetSubscriptionCatalogAsync());
    }
}