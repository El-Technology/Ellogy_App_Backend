﻿using AICommunicationService.BLL.Interfaces;
using AICommunicationService.Common.Models.AIRequest;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AICommunicationService.Api.Controllers
{
    /// <summary>
    /// This controller provides endpoints for communication with Chat GPT using various templates and methods.
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
        /// Endpoint for retrieving AI response as streaming using SignalR.
        /// </summary>
        /// <param name="streamRequest">Request params</param>
        /// <returns>Returns true if request is success</returns>
        [HttpPost]
        [Route("getSignalRStreamResponse")]
        public async Task<IActionResult> GetSignalRStreamResponse([FromBody] StreamRequest streamRequest)
        {
            var response = await _communicationService.StreamSignalRConversationAsync(streamRequest);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint for retrieving AI response as string.
        /// </summary>
        /// <param name="conversationRequest">Request params</param>
        /// <returns>Returns true if request is success</returns>
        [HttpPost]
        [Route("getChatResponse")]
        public async Task<IActionResult> GetChatResponse([FromBody] CreateConversationRequest conversationRequest)
        {
            var response = await _communicationService.ChatRequestAsync(conversationRequest);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint for retrieving AI response by Json Example.
        /// </summary>
        /// <param name="requestWithFunction"></param>
        /// <returns>Returns string data in Json</returns>
        [HttpPost]
        [Route("chatWithFunctions")]
        public async Task<IActionResult> GetChatWithFunctions([FromBody] CreateConversationRequest requestWithFunction)
        {
            var response = await _communicationService.ChatRequestWithFunctionAsync(requestWithFunction);
            return Ok(response);
        }

        [HttpPost]
        [Route("stream")]
        public async Task GetStream([FromBody] CreateConversationRequest createConversationRequest)
        {
            await _communicationService.ReturnStreamingAsync(createConversationRequest, async res =>
            {
                await Response.WriteAsync(res);
            });
        }
    }
}