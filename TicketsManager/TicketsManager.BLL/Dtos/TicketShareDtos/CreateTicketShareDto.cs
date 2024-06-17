using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class CreateTicketShareDto
{
    public Guid SharedUserId { get; set; }
    public SharePermissionEnum Permission { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid TicketId { get; set; }
}
