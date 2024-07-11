using Microsoft.AspNetCore.Mvc;
using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Interfaces;
using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;
using TicketsManager.DAL.Enums;

namespace TicketsManager.Api.Controllers;

/// <summary>
/// Represents the API endpoints for managing ticket messages.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TicketMessageController : ControllerBase
{
    private readonly ITicketMessageService _ticketMessageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TicketMessageController"/> class.
    /// </summary>
    /// <param name="ticketMessageService"></param>
    public TicketMessageController(ITicketMessageService ticketMessageService)
    {
        _ticketMessageService = ticketMessageService;
    }

    /// <summary>
    /// This method retrieves the user id from the JWT token
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private Guid GetUserIdFromToken() =>
        TokenParseHelper.GetUserId(User);

    /// <summary>
    /// Retrieves all messages associated with the specified ticket.
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="paginationRequest"></param>
    /// <param name="subStageEnum"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(PaginationResponseDto<MessageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("getMessages")]
    public async Task<IActionResult> GetTicketMessagesByTicketIdAsync(
        [FromQuery] Guid ticketId,
        [FromQuery] SubStageEnum? subStageEnum,
        [FromBody] PaginationRequestDto paginationRequest)
    {
        var messages = await _ticketMessageService
            .GetTicketMessagesByTicketIdAsync(ticketId, GetUserIdFromToken(), paginationRequest, subStageEnum);

        return Ok(messages);
    }

    /// <summary>
    /// Updates a message.
    /// </summary>
    /// <param name="messageRequestDto"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Route("updateMessage")]
    public async Task<IActionResult> UpdateMessageAsync(
               [FromBody] MessageResponseDto messageRequestDto,
               [FromQuery] Guid ticketId)
    {
        var message = await _ticketMessageService.UpdateMessageAsync(
            messageRequestDto, ticketId, GetUserIdFromToken());

        return Ok(message);
    }

    /// <summary>
    /// Updates a range of messages.
    /// </summary>
    /// <param name="messageResponseDtos"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(IEnumerable<MessageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Route("updateRangeMessages")]
    public async Task<IActionResult> UpdateRangeMessagesAsync(
               [FromBody] List<MessageResponseDto> messageResponseDtos,
               [FromQuery] Guid ticketId)
    {
        var messages = await _ticketMessageService.UpdateRangeMessagesAsync(
            messageResponseDtos, ticketId, GetUserIdFromToken());

        return Ok(messages);
    }

    /// <summary>
    /// Creates a new message.
    /// </summary>
    /// <param name="messageDto"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("createMessage")]
    public async Task<IActionResult> CreateMessageAsync(
        [FromBody] MessageDto messageDto,
        [FromQuery] Guid ticketId)
    {
        var message = await _ticketMessageService.CreateMessageAsync(
            messageDto, ticketId, GetUserIdFromToken());

        return Ok(message);
    }

    /// <summary>
    /// Creates a range of messages.
    /// </summary>
    /// <param name="messageDtos"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(IEnumerable<MessageResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Route("createRangeMessages")]
    public async Task<IActionResult> CreateRangeMessagesAsync(
        [FromBody] List<MessageDto> messageDtos,
        [FromQuery] Guid ticketId)
    {
        var messages = await _ticketMessageService.CreateRangeMessagesAsync(
            messageDtos, ticketId, GetUserIdFromToken());

        return Ok(messages);
    }

    /// <summary>
    /// Deletes a message.
    /// </summary>
    /// <param name="messageId"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    [Route("deleteMessage")]
    public async Task<IActionResult> DeleteMessageAsync(
        [FromQuery] Guid messageId,
        [FromQuery] Guid ticketId)
    {
        await _ticketMessageService.DeleteMessageAsync(
            messageId, GetUserIdFromToken(), ticketId);

        return Ok();
    }

    /// <summary>
    /// Deletes a range of messages.
    /// </summary>
    /// <param name="messageIds"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    [Route("deleteRangeMessages")]
    public async Task<IActionResult> DeleteRangeMessagesAsync(
        [FromBody] List<Guid> messageIds,
        [FromQuery] Guid ticketId)
    {
        await _ticketMessageService.DeleteRangeMessagesAsync(messageIds, GetUserIdFromToken(), ticketId);

        return Ok();
    }
}
