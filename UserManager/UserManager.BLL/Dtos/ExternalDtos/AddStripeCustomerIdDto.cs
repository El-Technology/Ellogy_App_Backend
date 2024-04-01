namespace UserManager.BLL.Dtos.ExternalDtos;
public class AddStripeCustomerIdDto
{
    public Guid UserId { get; set; }
    public string CustomerId { get; set; }
}
