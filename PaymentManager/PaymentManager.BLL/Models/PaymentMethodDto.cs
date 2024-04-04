namespace PaymentManager.BLL.Models;

public class PaymentMethodDto
{
    public string Type { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string CardBrand { get; set; } = string.Empty;
    public string Expires { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public bool Default { get; set; }
}