namespace AICommunicationService.BLL.Dtos;

public class DocumentResponseWithOwner : DocumentResponseDto
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? AvatarLink { get; set; }
}