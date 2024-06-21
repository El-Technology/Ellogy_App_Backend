using AICommunicationService.BLL.Exceptions;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using AICommunicationService.Common.Enums;

namespace AICommunicationService.BLL.Services.HttpServices;
public class TicketExternalHttpService : ITicketExternalHttpService
{
    private readonly HttpClient _httpClient;
    public TicketExternalHttpService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("TicketManager");
    }

    /// <inheritdoc cref="ITicketExternalHttpService.CheckUserAccessAsync(Guid, Guid, TicketCurrentStepEnum, SharePermissionEnum)"/>
    public async Task CheckUserAccessAsync(
        Guid ticketId,
        Guid userId,
        TicketCurrentStepEnum currentStepEnum,
        SharePermissionEnum requireSharePermissionEnum)
    {
        var result = await _httpClient.GetAsync(
            $"api/TicketExternal/check-user-access" +
                $"?ticketId={ticketId}" +
                $"&userId={userId}" +
                $"&currentStepEnum={currentStepEnum}" +
                $"&requireSharePermissionEnum={requireSharePermissionEnum}");

        if (!result.IsSuccessStatusCode)
            throw new ForbiddenException(userId);

        await Task.CompletedTask;
    }
}
