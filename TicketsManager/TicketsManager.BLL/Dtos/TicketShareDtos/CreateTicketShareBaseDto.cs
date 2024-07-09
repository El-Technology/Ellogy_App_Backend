using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketShareDtos;
public class CreateTicketShareBaseDto
{
    public TicketCurrentStepEnum? TicketCurrentStep { get; set; }
    public SubStageEnum? SubStageEnum { get; set; }
}
