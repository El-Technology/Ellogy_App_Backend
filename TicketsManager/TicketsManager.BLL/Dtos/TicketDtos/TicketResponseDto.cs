using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Dtos.NotificationDtos;
using TicketsManager.BLL.Dtos.TicketShareDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos;

/// <summary>
///     Represents the response data for a ticket.
/// </summary>
public class TicketResponseDto
{
    /// <summary>
    ///     The unique identifier of the ticket.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The title of the ticket.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     The description of the ticket.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The context of all conversation with bot.
    /// </summary>
    public string? Context { get; set; }

    /// <summary>
    ///     The comment associated with the ticket.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    ///     The date and time when the ticket was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    ///     The date and time when the ticket was last updated.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    ///     The status of the ticket.
    /// </summary>
    public TicketStatusEnum Status { get; set; }

    /// <summary>
    ///     Shows at which step the user is at
    /// </summary>
    public TicketCurrentStepEnum CurrentStep { get; set; }

    /// <summary>
    ///     Contains link json array with banners for report page
    /// </summary>
    public string? BannersJson { get; set; }

    /// <summary>
    ///     The collection of messages associated with the ticket.
    /// </summary>
    public List<MessageResponseDto> Messages { get; set; }

    /// <summary>
    ///     The notifications of the ticket with ids
    /// </summary>
    public List<NotificationFullDto> Notifications { get; set; }

    /// <summary>
    ///    The business impact of the ticket.
    /// </summary>
    public string? BusinessImpact { get; set; }

    /// <summary>
    ///  Returns information about user permissions for this ticket or null if user is owner
    /// </summary>
    public List<PermissionDto>? Permissions { get; set; }
}