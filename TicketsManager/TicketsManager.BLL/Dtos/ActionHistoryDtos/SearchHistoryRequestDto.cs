using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.ActionHistoryDtos
{
    public class SearchHistoryRequestDto
    {
        public TicketCurrentStepEnum TicketCurrentStepEnum { get; set; }
        public PaginationRequestDto Pagination { get; set; }
    }
}
