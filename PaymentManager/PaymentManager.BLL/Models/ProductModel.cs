namespace PaymentManager.BLL.Models;

public class ProductModel
{
    public string Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string PriceId { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new List<string>();
}