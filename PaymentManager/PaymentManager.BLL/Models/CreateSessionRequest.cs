namespace PaymentManager.BLL.Models;

public class CreateSessionRequest : SignalRModel
{
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}