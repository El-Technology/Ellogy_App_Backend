using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.ActionHistoryDtos
{
    public class CreateActionHistoryDto
    {
        public Guid TicketId { get; set; }
        public ActionHistoryEnum ActionHistoryEnum { get; set; }
        public TicketCurrentStepEnum TicketCurrentStepEnum { get; set; }
        public string? UserEmail { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
