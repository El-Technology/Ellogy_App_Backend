using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class UpdateTicketShareDto
{
    public SharePermissionEnum Permission { get; set; }
    public DateTime? RevokedAt { get; set; }
}
