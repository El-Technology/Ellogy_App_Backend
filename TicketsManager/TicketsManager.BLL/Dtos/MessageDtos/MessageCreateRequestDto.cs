namespace TicketsManager.BLL.Dtos.MessageDtos
{
    /// <summary>
    /// Represents the request data for creating a new message.
    /// </summary>
    public class MessageCreateRequestDto
    {
        /// <summary>
        /// The unique identifier of the ticket associated with the message.
        /// </summary>
        public Guid TicketId { get; set; }

        /// <summary>
        /// The sender of the message.
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Content { get; set; }
    }
}
