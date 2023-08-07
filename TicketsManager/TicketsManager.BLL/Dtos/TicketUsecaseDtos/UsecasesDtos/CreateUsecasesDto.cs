namespace TicketsManager.BLL.Dtos.TicketUsecaseDtos.UsecasesDtos
{
    public class CreateUsecasesDto
    {
        public UsecaseDto Usecase { get; set; }
        public Guid TicketId { get; set; }
    }
}
