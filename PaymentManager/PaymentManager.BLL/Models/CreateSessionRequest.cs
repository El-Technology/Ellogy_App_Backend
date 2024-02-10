namespace PaymentManager.BLL.Models;

public class CreateSessionRequest : SignalRModel
{
    public string SuccessUrl { get; set; } = null!;
    public string CancelUrl { get; set; } = null!;
}