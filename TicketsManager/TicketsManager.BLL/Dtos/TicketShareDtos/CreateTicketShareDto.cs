using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class CreateTicketShareDto : CreateTicketShareBaseDto
{
    public Guid SharedUserId { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid TicketId { get; set; }
    public SharePermissionEnum Permission { get; set; }
}
