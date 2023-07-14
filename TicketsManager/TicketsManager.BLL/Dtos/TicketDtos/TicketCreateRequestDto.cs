using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos
{
    /// <summary>
    /// Represents the request data for creating a new ticket.
    /// </summary>
    public class TicketCreateRequestDto
    {
        /// <summary>
        /// The title of the ticket.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the ticket.
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// The summary that generate ChatGPT.
        /// </summary>
        public string? Summary { get; set; }
        
        /// <summary>
        /// The context of all conversation with bot.
        /// </summary>
        public string? Context { get; set; }

        /// <summary>
        /// The date and time when the ticket is created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The status of the ticket.
        /// </summary>
        public TicketStatusEnum Status { get; set; }
        
        /// <summary>
        /// The messages list of ticket.
        /// </summary>
        public List<MessageDto> Messages { get; set; }
    }
}
