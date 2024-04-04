namespace PaymentManager.BLL.Models;

public class PaymentObject
{
    public string Id { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string DownloadLink { get; set; } = string.Empty;
}