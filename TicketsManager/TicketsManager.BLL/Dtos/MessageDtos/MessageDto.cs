using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.MessageDtos;

public class MessageDto
{
    /// <summary>
    /// The sender of the message.
    /// </summary>
    public string Sender { get; set; }

    /// <summary>
    /// The content of the message.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The time when message was sent.
    /// </summary>
    public DateTime SendTime { get; set; }

    /// <summary>
    /// Represents the type and the state of message.
    /// </summary>
    public ActionDto? Action { get; set; }

    /// <summary>
    /// Shows at which stage the message was sent
    /// </summary>
    public MessageStageEnum? Stage { get; set; }

    /// <summary>
    /// Shows at which sub - stage the message was sent
    /// </summary>
    public SubStageEnum? SubStage { get; set; }
}
