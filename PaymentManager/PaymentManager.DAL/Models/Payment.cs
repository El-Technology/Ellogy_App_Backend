namespace PaymentManager.DAL.Models;

public class Payment
{
    public Guid Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public string InvoiceId { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public int AmountOfPoints { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool UpdatedBallance { get; set; }
    public Guid UserId { get; set; }
}