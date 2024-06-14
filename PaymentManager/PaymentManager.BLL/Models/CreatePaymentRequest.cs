namespace PaymentManager.BLL.Models;

public class CreatePaymentRequest : CreateSessionRequest
{
    public int AmountOfTickets { get; set; }
}