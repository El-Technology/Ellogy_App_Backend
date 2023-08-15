namespace UserManager.Common.Models.NotificationModels
{
    public class NotificationModel
    {
        public Dictionary<string, string> MetaData { get; set; }
        public List<string>? BlobUrls { get; set; }
        public string Consumer { get; set; }
        public NotificationTypeEnum Type { get; set; }
        public NotificationWayEnum Way { get; set; }
    }
}
