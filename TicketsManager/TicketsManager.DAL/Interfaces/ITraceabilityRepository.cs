using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Interfaces;

/// <summary>
/// Interface for managing traceability matrix data operations.
/// </summary>
public interface ITraceabilityRepository
{
    /// <summary>
    /// Retrieve raw traceability data for a specific ticket using the SQL view.
    /// </summary>
    /// <param name="ticketId">The unique identifier of the ticket.</param>
    /// <returns>Returns a collection of traceability matrix rows from the database view.</returns>
    Task<IEnumerable<TraceabilityMatrixView>> GetTraceabilityMatrixViewAsync(Guid ticketId);
}