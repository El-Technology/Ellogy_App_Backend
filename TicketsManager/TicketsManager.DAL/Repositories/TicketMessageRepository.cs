using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Repositories;
public class TicketMessageRepository : ITicketMessageRepository
{
    private readonly TicketsManagerDbContext _context;
    public TicketMessageRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    private IQueryable<Message> GetMessagesByUserIdQuery(Guid userId)
    {
        return _context.Messages
            .Where(msg =>
                    msg.Ticket.UserId == userId ||
                    msg.Ticket.TicketShares.Any(ts =>
                        ts.SharedUserId == userId &&
                        (
                            ts.TicketCurrentStep == Enums.TicketCurrentStepEnum.General ||
                            ts.TicketCurrentStep == null ||
                            ts.TicketCurrentStep == Enums.TicketCurrentStepEnum.Report
                        ) &&
                        (
                            ts.TicketCurrentStep == null ||
                            ts.SubStageEnum == null ||
                            ts.SubStageEnum == msg.SubStage ||
                            msg.SubStage == Enums.SubStageEnum.FunctionalRequirements
                        )
                    )
                )
                .OrderBy(a => a.SendTime);
    }

    /// <inheritdoc cref="ITicketMessageRepository.GetMessageAsync" />
    public Task<Message?> GetMessageAsync(Guid messageId, Guid userId)
    {
        return GetMessagesByUserIdQuery(userId)
            .FirstOrDefaultAsync(msg => msg.Id == messageId);
    }

    /// <inheritdoc cref="ITicketMessageRepository.GetRangeMessagesAsync" />
    public async Task<List<Message>?> GetRangeMessagesAsync(
        List<Guid> messageId, Guid userId)
    {
        return await GetMessagesByUserIdQuery(userId)
            .Where(msg => messageId.Contains(msg.Id))
            .ToListAsync();
    }

    /// <inheritdoc cref="ITicketMessageRepository.GetTicketMessagesByTicketIdAsync" />
    public Task<PaginationResponseDto<Message>> GetTicketMessagesByTicketIdAsync(
        Guid ticketId,
        Guid userId,
        PaginationRequestDto paginationRequest,
        SubStageEnum? subStageEnum)
    {
        return GetMessagesByUserIdQuery(userId)
            .Where(msg => msg.TicketId == ticketId &&
                (subStageEnum == null || msg.SubStage == subStageEnum))
            .GetFinalResultAsync(paginationRequest);
    }

    /// <inheritdoc cref="ITicketMessageRepository.UpdateMessageAsync" />
    public Task UpdateMessageAsync(Message message)
    {
        _context.Messages.Update(message);
        return _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketMessageRepository.UpdateRangeMessagesAsync" />
    public async Task UpdateRangeMessagesAsync(List<Message> messages)
    {
        _context.Messages.UpdateRange(messages);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketMessageRepository.CreateMessageAsync" />
    public async Task CreateMessageAsync(Message message)
    {
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketMessageRepository.CreateRangeMessagesAsync" />
    public async Task CreateRangeMessagesAsync(List<Message> messages)
    {
        await _context.Messages.AddRangeAsync(messages);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketMessageRepository.DeleteMessageAsync" />
    public async Task DeleteMessageAsync(Message message)
    {
        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketMessageRepository.DeleteRangeMessagesAsync" />
    public async Task DeleteRangeMessagesAsync(List<Message> messages)
    {
        _context.Messages.RemoveRange(messages);
        await _context.SaveChangesAsync();
    }
}