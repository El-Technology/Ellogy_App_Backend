namespace TicketsManager.BLL.Dtos.MessageDtos
{
    /// <summary>
    /// Represents the response data for a message.
    /// </summary>
    public class MessageResponseDto : MessageDto
    {
        /// <summary>
        /// The unique identifier of the message.
        /// </summary>
        public Guid Id { get; set; }
    }
}
