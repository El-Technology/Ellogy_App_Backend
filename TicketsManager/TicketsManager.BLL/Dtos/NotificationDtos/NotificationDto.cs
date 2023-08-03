namespace TicketsManager.BLL.Dtos.NotificationDtos
{
    public class NotificationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Sms { get; set; }
        public bool Email { get; set; }
        public bool Push { get; set; }
    }
}
