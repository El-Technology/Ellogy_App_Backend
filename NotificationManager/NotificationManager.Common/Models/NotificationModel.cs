namespace NotificationManager.Common.Models;

public class NotificationModel
{
    public Dictionary<string, string> MetaData { get; set; } = new();
    public List<string>? BlobUrls { get; set; }
    public string? Consumer { get; set; }
    public NotificationTypeEnum Type { get; set; }
    public NotificationWayEnum Way { get; set; }
}
