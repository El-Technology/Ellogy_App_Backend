using TicketsManager.BLL.Interfaces;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Interfaces;

namespace TicketsManager.BLL.Services;
public class TicketShareExternalService : ITicketShareExternalService
{
    private readonly ITicketShareRepository _ticketShareRepository;

    public TicketShareExternalService(ITicketShareRepository ticketShareRepository)
    {
        _ticketShareRepository = ticketShareRepository;
    }

    public async Task CheckIfUserHaveAccessToComponentByTicketIdAsync(
        Guid ticketId,
        Guid userId,
        TicketCurrentStepEnum currentStepEnum,
        SharePermissionEnum requireSharePermissionEnum)
    {
        await _ticketShareRepository.CheckIfUserHaveAccessToComponentAsync(
            ticketId,
            userId,
            currentStepEnum,
            requireSharePermissionEnum);
    }
}
