using PaymentManager.BLL.Interfaces;
using PaymentManager.BLL.Models;
using PaymentManager.Common.Constants;
using Stripe;

namespace PaymentManager.BLL.Services
{
    public class ProductCatalogService : StripeBaseService, IProductCatalogService
    {
        /// <inheritdoc cref="IProductCatalogService.GetSubscriptionCatalogAsync"/>
        public async Task<IEnumerable<ProductModel>> GetSubscriptionCatalogAsync()
        {
            var allProducts = await GetProductService().ListAsync(new()
            {
                Active = true,
                Expand = new List<string> { "data.default_price" }
            });

            var productModels = new List<ProductModel>();

            foreach (var product in allProducts)
            {
                var price = product.DefaultPrice != null
                    ? $"{product.DefaultPrice.UnitAmountDecimal / Constants.PriceInCents} {product.DefaultPrice.Currency}"
                    : "Price was not found, contact with our service please";

                var productModel = new ProductModel
                {
                    Name = product.Name,
                    Price = price,
                    Description = product.Description,
                    ProductId = product.Id,
                    PriceId = product.DefaultPrice?.Id
                };

                productModels.Add(productModel);
            }

            return productModels;
        }


        /// <inheritdoc cref="IProductCatalogService.GetProductAsync(string)"/>
        public async Task<ProductModel> GetProductAsync(string productId)
        {
            var product = await GetProductService().GetAsync(productId, new ProductGetOptions { Expand = new List<string> { "default_price" } });

            var price = product.DefaultPrice != null
                ? $"{product.DefaultPrice.UnitAmountDecimal / Constants.PriceInCents} {product.DefaultPrice.Currency}"
                : "0";

            return new ProductModel
            {
                Name = product.Name,
                Price = price,
                Description = product.Description,
                ProductId = product.Id,
                PriceId = product.DefaultPrice?.Id
            };
        }
    }
}

