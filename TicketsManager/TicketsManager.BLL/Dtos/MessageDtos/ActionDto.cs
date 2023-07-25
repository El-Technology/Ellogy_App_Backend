using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.MessageDtos
{
    public class ActionDto
    {
        public MessageActionStateEnum State { get; set; }
        public MessageActionTypeEnum Type { get; set; }
    }
}
