using Stripe;

namespace PaymentManager.BLL.Services
{
    public class ProductCatalogService
    {
        public async IAsyncEnumerable<object> GetSubscriptionCatalogAsync()
        {
            var productService = new ProductService();
            var allProducts = await productService.ListAsync(new ProductListOptions { Active = true, Expand = new List<string> { "data.default_price" } });
            foreach (var product in allProducts)
            {
                yield return new
                {
                    Name = product.Name,
                    Price = $"{product.DefaultPrice.UnitAmountDecimal / 100} {product.DefaultPrice.Currency}",
                    Description = product.Description,
                    ProductId = product.Id,
                    PriceId = product.DefaultPrice.Id
                };
            }
        }
    }
}
