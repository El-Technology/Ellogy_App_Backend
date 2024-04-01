namespace AICommunicationService.DAL.Models;

public class DocumentSharing
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Document Document { get; set; }
    public Guid DocumentId { get; set; }
}