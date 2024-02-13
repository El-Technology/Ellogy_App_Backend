namespace PaymentManager.BLL.Models;

public class PaymentObject
{
    public string Id { get; set; }
    public string? Product { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Status { get; set; }
    public string? DownloadLink { get; set; }
}