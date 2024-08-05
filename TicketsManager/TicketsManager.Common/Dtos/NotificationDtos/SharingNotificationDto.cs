namespace TicketsManager.Common.Dtos.NotificationDtos;
public class SharingNotificationDto
{
    public string ConsumerEmail { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string OwnerFistName { get; set; } = string.Empty;
    public string OwnerLastName { get; set; } = string.Empty;
    public string TicketTitle { get; set; } = string.Empty;
    public string AccessTo { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
}
