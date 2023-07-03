using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos
{
    /// <summary>
    /// Represents the request data for updating a ticket.
    /// </summary>
    public class TicketUpdateRequestDto
    {
        /// <summary>
        /// The updated title of the ticket.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The updated description of the ticket.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The updated summary of the ticket.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// The updated comment associated with the ticket.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// The updated status of the ticket.
        /// </summary>
        public TicketStatusEnum Status { get; set; }
        
        /// <summary>
        /// The messages list of ticket.
        /// </summary>
        public List<MessageDto> Messages { get; set; }
    }
}
