using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.DAL.Dtos;
public class GetMessageDto
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public PaginationRequestDto PaginationRequest { get; set; } = new();
    public SubStageEnum? SubStageEnum { get; set; }
    public TicketCurrentStepEnum MessageStage { get; set; }
}
