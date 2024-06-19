using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class PermissionDto
{
    public Guid SharedUserId { get; set; }
    public DateTime? RevokedAt { get; set; }
    public SharePermissionEnum Permission { get; set; }
    public TicketCurrentStepEnum? TicketCurrentStep { get; set; }
    public SubStageEnum? SubStageEnum { get; set; }
}
