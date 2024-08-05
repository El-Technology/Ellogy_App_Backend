using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class CreateManyTicketShareDto
{
    public Guid SharedUserId { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid TicketId { get; set; }
    public SharePermissionEnum Permission { get; set; }
    public List<CreateTicketShareBaseDto> CreateTicketShareDtos { get; set; }
}
