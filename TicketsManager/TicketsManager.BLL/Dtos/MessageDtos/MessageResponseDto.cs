namespace TicketsManager.BLL.Dtos.MessageDtos
{
    /// <summary>
    /// Represents the response data for a message.
    /// </summary>
    public class MessageResponseDto
    {
        /// <summary>
        /// The unique identifier of the message.
        /// </summary>
        public Guid Id { get; set; }

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
