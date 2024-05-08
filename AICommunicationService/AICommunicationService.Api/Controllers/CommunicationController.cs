using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common;
using AICommunicationService.Common.Enums;
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

    public CommunicationController(ICommunicationService communicationService)
    {
        _communicationService = communicationService;
    }

    /// <summary>
    ///     This method checks the user plan and sets the request parameters accordingly.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    private T CheckUserPlan<T>(T request) where T : CreateConversationRequest
    {
        if (User.HasClaim(JwtOptions.ACCOUNT_PLAN, AccountPlan.Basic.ToString()))
            return request;

        request.FileName = null;
        request.UseRAG = false;

        return request;
    }

    /// <summary>
    ///     This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken()
    {
        var status = Guid.TryParse(User.FindFirst(JwtOptions.USER_ID_CLAIM_NAME)?.Value, out var userId);
        if (!status)
            throw new Exception("Taking user id error, try again later");

        return userId;
    }

    /// <summary>
    ///    Endpoint for retrieving AI response as streaming.
    /// </summary>
    /// <param name="conversationRequest"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("getStreamResponse")]
    public async Task GetStreamResponse([FromBody] CreateConversationRequest conversationRequest)
    {
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Content-Type", "text/event-stream");
        await _communicationService.StreamRequestAsync(GetUserIdFromToken(), CheckUserPlan(conversationRequest),
            async response =>
            {
                await Response.WriteAsync($"{response}/n/n");
                await Response.Body.FlushAsync();
            });
    }

    /// <summary>
    ///     Endpoint for retrieving AI response as string.
    /// </summary>
    /// <param name="conversationRequest">Request params</param>
    /// <returns>Returns true if request is success</returns>
    [HttpPost]
    [Route("getChatResponse")]
    public async Task<IActionResult> GetChatResponse([FromBody] CreateConversationRequest conversationRequest)
    {
        var response =
            await _communicationService.ChatRequestAsync(GetUserIdFromToken(), CheckUserPlan(conversationRequest));
        return Ok(response);
    }

    /// <summary>
    ///     Endpoint for retrieving AI response by Json Example.
    /// </summary>
    /// <param name="requestWithFunction"></param>
    /// <returns>Returns string data in Json</returns>
    [HttpPost]
    [Route("chatWithFunctions")]
    public async Task<IActionResult> GetChatWithFunctions([FromBody] CreateConversationRequest requestWithFunction)
    {
        var response =
            await _communicationService.ChatRequestWithFunctionAsync(GetUserIdFromToken(),
                CheckUserPlan(requestWithFunction));
        return Ok(response);
    }
}