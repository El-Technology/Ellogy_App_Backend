using AutoMapper;
using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using Stripe;

namespace PaymentManager.BLL.Services;

/// <summary>
///     This class contains methods for product catalog related operations
/// </summary>
public class ProductCatalogService : StripeBaseService, IProductCatalogService
{
    private readonly IMapper _mapper;

    public ProductCatalogService(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc cref="IProductCatalogService.GetSubscriptionCatalogAsync" />
    public async Task<IEnumerable<ProductModel>> GetSubscriptionCatalogAsync()
    {
        var allProducts = await GetProductService().ListAsync(new ProductListOptions
        {
            Active = true,
            Expand = new List<string> { "data.default_price" }
        });

        return _mapper.Map<List<ProductModel>>(allProducts);
    }


    /// <inheritdoc cref="IProductCatalogService.GetProductAsync(string)" />
    public async Task<ProductModel> GetProductAsync(string productId)
    {
        var product = await GetProductService().GetAsync(productId,
            new ProductGetOptions { Expand = new List<string> { "default_price" } });

        return _mapper.Map<ProductModel>(product);
    }

    /// <inheritdoc cref="IProductCatalogService.GetProductByNameAsync(string)" />
    public async Task<ProductModel> GetProductByNameAsync(string productName)
    {
        var product = (await GetProductService().SearchAsync(new ProductSearchOptions
        {
            Expand = new List<string> { "data.default_price" },
            Query = $"active:'true' AND name~'{productName}'"
        })).FirstOrDefault() ?? throw new Exception($"Product with name {productName} was not found");

        return _mapper.Map<ProductModel>(product);
    }
}