using TicketsManager.BLL.Dtos.TraceabilityDtos;

namespace TicketsManager.BLL.Interfaces;

/// <summary>
/// Interface for managing traceability matrix operations.
/// </summary>
public interface ITraceabilityService
{
    /// <summary>
    /// Retrieve the traceability matrix for a specific ticket.
    /// </summary>
    /// <param name="ticketId">The unique identifier of the ticket.</param>
    /// <param name="userIdFromToken">User id taken from jwt token for permission validation.</param>
    /// <returns>Returns the traceability matrix containing user stories, scenarios, acceptance criteria, and linked tests.</returns>
    Task<TraceabilityMatrixResponseDto> GetTraceabilityMatrixAsync(Guid ticketId, Guid userIdFromToken);
}