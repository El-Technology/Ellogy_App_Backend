using AICommunicationService.BLL.Dtos;
using AICommunicationService.BLL.Interfaces;
using AICommunicationService.BLL.Interfaces.HttpInterfaces;
using AICommunicationService.Common;
using AICommunicationService.Common.Enums;
using AICommunicationService.Common.Helpers;
using AICommunicationService.Common.Models.AIRequest;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Controllers;

/// <summary>
///     This controller provides endpoints for communication with Chat GPT using various templates and methods.
/// </summary>
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
[ApiController]
public class CommunicationController : ControllerBase
{
    private readonly ICommunicationService _communicationService;
    private readonly ITicketExternalHttpService _ticketExternalHttpService;

    /// <summary>
    ///    Constructor for CommunicationController
    /// </summary>
    /// <param name="communicationService"></param>
    /// <param name="ticketExternalHttpService"></param>
    public CommunicationController(
        ICommunicationService communicationService,
        ITicketExternalHttpService ticketExternalHttpService)
    {
        _communicationService = communicationService;
        _ticketExternalHttpService = ticketExternalHttpService;
    }

    /// <summary>
    ///     This method checks the user plan and sets the request parameters accordingly.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    private ConversationRequestDto CheckUserPlan<T>(T request) where T : CreateConversationRequest
    {
        var plan = Enum.Parse<AccountPlan>(TokenParseHelper.GetValueFromJwt(JwtOptions.ACCOUNT_PLAN, User));

        if (plan != AccountPlan.Starter)
        {
            request.FileName = null;
            request.UseRAG = false;
        }

        return new ConversationRequestDto
        {
            CreateConversationRequest = request,
            AccountPlan = plan
        };
    }

    /// <summary>
    ///    This method checks if the user has access to the ticket.
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="userId"></param>
    /// <param name="currentStepEnum"></param>
    /// <returns></returns>
    private async Task CheckUserAccessAsync(
        Guid ticketId, Guid userId, TicketCurrentStepEnum currentStepEnum)
    {
        var requireSharePermissionEnum = SharePermissionEnum.ReadWrite;
        await _ticketExternalHttpService.CheckUserAccessAsync(
            ticketId, userId, currentStepEnum, requireSharePermissionEnum);
    }


    /// <summary>
    ///     This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        Guid.Parse(TokenParseHelper.GetValueFromJwt(JwtOptions.UserIdClaimName, User));

    /// <summary>
    ///    Endpoint for retrieving AI response as streaming.
    /// </summary>
    /// <param name="conversationRequest"></param>
    /// <param name="ticketId"></param>
    /// <param name="ticketCurrentStep"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("getStreamResponse")]
    public async Task GetStreamResponse(
        [FromBody] CreateConversationRequest conversationRequest,
        [FromQuery] Guid? ticketId,
        [FromQuery] TicketCurrentStepEnum? ticketCurrentStep)
    {
        if (ticketId is not null)
            await CheckUserAccessAsync(ticketId ?? Guid.Empty,
                GetUserIdFromToken(),
                ticketCurrentStep ?? TicketCurrentStepEnum.General);

        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Content-Type", "text/event-stream");
        await _communicationService.StreamRequestAsync(GetUserIdFromToken(), CheckUserPlan(conversationRequest),
            async response =>
            {
                await Response.WriteAsync($"{response}\n");
                await Response.Body.FlushAsync();
            });
    }

    /// <summary>
    ///     Endpoint for retrieving AI response as string.
    /// </summary>
    /// <param name="conversationRequest">Request params</param>
    /// <param name="ticketId"></param>
    /// <param name="ticketCurrentStep"></param>
    /// <returns>Returns true if request is success</returns>
    [HttpPost]
    [Route("getChatResponse")]
    public async Task<IActionResult> GetChatResponse(
        [FromBody] CreateConversationRequest conversationRequest,
        [FromQuery] Guid? ticketId,
        [FromQuery] TicketCurrentStepEnum? ticketCurrentStep)
    {
        if (ticketId is not null)
            await CheckUserAccessAsync(ticketId ?? Guid.Empty,
                GetUserIdFromToken(),
                ticketCurrentStep ?? TicketCurrentStepEnum.General);

        var response =
            await _communicationService.ChatRequestAsync(GetUserIdFromToken(), CheckUserPlan(conversationRequest));
        return Ok(response);
    }

    /// <summary>
    ///     Endpoint for retrieving AI response by Json Example.
    /// </summary>
    /// <param name="requestWithFunction"></param>
    /// <param name="ticketId"></param>
    /// <param name="ticketCurrentStep"></param>
    /// <returns>Returns string data in Json</returns>
    [HttpPost]
    [Route("chatWithFunctions")]
    public async Task<IActionResult> GetChatWithFunctions(
        [FromBody] CreateConversationRequest requestWithFunction,
        [FromQuery] Guid? ticketId,
        [FromQuery] TicketCurrentStepEnum? ticketCurrentStep)
    {
        if (ticketId is not null)
            await CheckUserAccessAsync(ticketId ?? Guid.Empty,
                GetUserIdFromToken(),
                ticketCurrentStep ?? TicketCurrentStepEnum.General);

        var response =
            await _communicationService.ChatRequestWithFunctionAsync(GetUserIdFromToken(),
                CheckUserPlan(requestWithFunction));
        return Ok(response);
    }
}