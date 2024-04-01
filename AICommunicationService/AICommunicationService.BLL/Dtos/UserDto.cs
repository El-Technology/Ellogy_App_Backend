namespace AICommunicationService.BLL.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? AvatarLink { get; set; }
    public int TotalPurchasedPoints { get; set; }
}