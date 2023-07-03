namespace TicketsManager.BLL.Dtos.MessageDtos
{
    /// <summary>
    /// Represents the request data for creating a new message.
    /// </summary>
    public class MessagesCreateRequestDto
    {
        /// <summary>
        /// The unique identifier of the ticket associated with the message.
        /// </summary>
        public Guid TicketId { get; set; }
        
        /// <summary>
        /// The list of messages for ticket.
        /// </summary>
        public List<MessageDto> Messages { get; set; }
    }
}
