using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class GetTicketShareDto
{
    public Guid Id { get; set; }
    public UserDto UserDto { get; set; } = null!;
    public DateTime GivenAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid TicketId { get; set; }

    public SharePermissionEnum Permission { get; set; }
    public TicketCurrentStepEnum? TicketCurrentStep { get; set; }
    public SubStageEnum? SubStageEnum { get; set; }
}
