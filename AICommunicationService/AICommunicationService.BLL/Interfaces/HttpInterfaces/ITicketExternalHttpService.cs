using AICommunicationService.Common.Enums;

namespace AICommunicationService.BLL.Interfaces.HttpInterfaces;
public interface ITicketExternalHttpService
{
    /// <summary>
    /// Check if user has access to the ticket
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <param name="currentStepEnum"></param>
    /// <param name="requireSharePermissionEnum"></param>
    /// <returns></returns>
    Task CheckUserAccessAsync(Guid ticketId,
        Guid userId,
        TicketCurrentStepEnum currentStepEnum,
        SharePermissionEnum requireSharePermissionEnum);
}