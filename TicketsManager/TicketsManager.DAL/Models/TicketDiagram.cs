namespace TicketsManager.DAL.Models
{
    public class TicketDiagram
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PictureLink { get; set; }
        public string PictureLinkPng { get; set; }

        public Usecase Usecase { get; set; }
        public Guid UsecaseId { get; set; }
    }
}
