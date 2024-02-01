using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using Stripe;

namespace PaymentManager.BLL.Services
{
    public class ProductCatalogService : IProductCatalogService
    {
        /// <inheritdoc cref="IProductCatalogService.GetSubscriptionCatalogAsync"/>
        public async IAsyncEnumerable<ProductModel> GetSubscriptionCatalogAsync()
        {
            var productService = new ProductService();
            var allProducts = await productService.ListAsync(new ProductListOptions { Active = true, Expand = new List<string> { "data.default_price" } });
            foreach (var product in allProducts)
            {
                yield return new ProductModel
                {
                    Name = product.Name,
                    Price = $"{product.DefaultPrice.UnitAmountDecimal / Constants.PriceInCents} {product.DefaultPrice.Currency}",
                    Description = product.Description,
                    ProductId = product.Id,
                    PriceId = product.DefaultPrice.Id
                };
            }
        }

        /// <inheritdoc cref="IProductCatalogService.GetProductAsync(string)"/>
        public async Task<ProductModel> GetProductAsync(string productId)
        {
            var productService = new ProductService();
            var product = await productService.GetAsync(productId, new ProductGetOptions { Expand = new List<string> { "default_price" } });

            return new ProductModel
            {
                Name = product.Name,
                Price = $"{product.DefaultPrice.UnitAmountDecimal / Constants.PriceInCents} {product.DefaultPrice.Currency}",
                Description = product.Description,
                ProductId = product.Id,
                PriceId = product.DefaultPrice.Id
            };
        }
    }
}

