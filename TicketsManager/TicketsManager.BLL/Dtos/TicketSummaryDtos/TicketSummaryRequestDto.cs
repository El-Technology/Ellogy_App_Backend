using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketSummaryDtos
{
    public class TicketSummaryRequestDto
    {
        public string Data { get; set; }
        public bool IsPotential { get; set; }

        /// <summary>
        /// Shows at which sub - stage the message was sent
        /// </summary>
        public SubStageEnum? SubStage { get; set; }
    }
}
