using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Interfaces;
public interface ITicketShareExternalService
{
    /// <summary>
    /// Check if user have access to component by ticket id
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <param name="currentStepEnum"></param>
    /// <param name="requireSharePermissionEnum"></param>
    /// <returns></returns>
    Task CheckIfUserHaveAccessToComponentByTicketIdAsync(
        Guid ticketId, Guid userId, TicketCurrentStepEnum currentStepEnum, SharePermissionEnum requireSharePermissionEnum);
}