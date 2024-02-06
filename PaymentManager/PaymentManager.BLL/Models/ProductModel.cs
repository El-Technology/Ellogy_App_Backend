using Stripe;

namespace PaymentManager.BLL.Models
{
    public class ProductModel
    {
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string ProductId { get; set; }
        public string? PriceId { get; set; }
        public List<string>? Features { get; set; }
    }
}
