using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos
{
    /// <summary>
    /// Represents the response data for a ticket.
    /// </summary>
    public class TicketResponseDto
    {
        /// <summary>
        /// The unique identifier of the ticket.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The title of the ticket.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the ticket.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The summary of the ticket.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// The comment associated with the ticket.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// The date and time when the ticket was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The date and time when the ticket was last updated.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// The status of the ticket.
        /// </summary>
        public TicketStatusEnum Status { get; set; }

        /// <summary>
        /// The collection of messages associated with the ticket.
        /// </summary>
        public ICollection<MessageResponseDto> TicketMessages { get; set; } = new List<MessageResponseDto>();
    }
}
