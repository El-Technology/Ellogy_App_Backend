namespace PaymentManager.BLL.Models
{
    public class PaymentObject
    {
        public string? Product { get; set; }
        public DateTime Date { get; set; }
        public float Amount { get; set; }
        public string? Status { get; set; }
        public string? DownloadLink { get; set; }
    }
}
