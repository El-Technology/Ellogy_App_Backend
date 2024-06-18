using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class GetTicketShareDto
{
    public Guid Id { get; set; }
    public UserDto UserDto { get; set; } = null!;
    public SharePermissionEnum Permission { get; set; }
    public DateTime GivenAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid TicketId { get; set; }
}
