namespace PaymentManager.BLL.Models;

public class PaymentMethod
{
    public string? Type { get; set; }
    public string? Id { get; set; }
    public string? CardBrand { get; set; }
    public string? Expires { get; set; }
    public string? Last4 { get; set; }
    public bool Default { get; set; }
}