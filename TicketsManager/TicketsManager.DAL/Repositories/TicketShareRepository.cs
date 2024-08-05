using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models.TicketModels;

namespace TicketsManager.DAL.Repositories;
public class TicketShareRepository : ITicketShareRepository
{
    private readonly TicketsManagerDbContext _context;
    private const int stepIncreaserNumber = 1;

    public TicketShareRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="ITicketShareRepository.CheckIfUserHaveAccessToSubStageAsync" />
    public async Task CheckIfUserHaveAccessToSubStageStrictAsync(
        Guid ticketId, Guid userId, SubStageEnum? subStageEnum, SharePermissionEnum requireSharePermissionEnum)
    {
        var ticket = await _context.Tickets
            .Include(e => e.TicketShares.
                Where(a =>
                    a.TicketId == ticketId &&
                    a.SharedUserId == userId &&
                    (
                        a.RevokedAt > DateTime.UtcNow ||
                        a.RevokedAt == null
                    ) &&
                    a.Permission >= requireSharePermissionEnum &&
                    (
                        (
                            a.SubStageEnum == subStageEnum &&
                            a.TicketCurrentStep == TicketCurrentStepEnum.Notifications
                        ) ||
                        (
                            a.SubStageEnum == subStageEnum &&
                            a.TicketCurrentStep == TicketCurrentStepEnum.General
                        ) ||
                        a.TicketCurrentStep == null ||
                        (
                            a.SubStageEnum == SubStageEnum.FunctionalRequirements &&
                            requireSharePermissionEnum == SharePermissionEnum.Read
                        ) ||
                        (
                            a.SubStageEnum == null &&
                            a.TicketCurrentStep == TicketCurrentStepEnum.General
                        )
                     )
                )
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == ticketId);

        if (ticket is not null && ticket.UserId == userId)
            return;
        if (ticket is null || ticket.TicketShares.Count == 0)
            throw new ForbiddenException(userId);
    }

    //Need to remove after front-end part ready and move to strict method 
    /// <inheritdoc cref="ITicketShareRepository.CheckIfUserHaveAccessToComponentAsync" />
    public async Task CheckIfUserHaveAccessToComponentAsync(
        Guid ticketId, Guid userId, TicketCurrentStepEnum currentStepEnum, SharePermissionEnum requireSharePermissionEnum)
    {
        var ticket = await _context.Tickets
            .Include(e => e.TicketShares
                .Where(a => a.TicketId == ticketId &&
                       a.SharedUserId == userId &&
                       (a.RevokedAt > DateTime.UtcNow || a.RevokedAt == null) &&
                       a.Permission >= requireSharePermissionEnum &&
                          (
                            a.TicketCurrentStep == null ||
                            a.TicketCurrentStep == currentStepEnum ||
                            (
                                requireSharePermissionEnum == SharePermissionEnum.Read &&
                                a.TicketCurrentStep == currentStepEnum + stepIncreaserNumber
                            ) ||
                            (
                                currentStepEnum == TicketCurrentStepEnum.General &&
                                a.TicketCurrentStep == TicketCurrentStepEnum.Notifications &&
                                a.Permission >= SharePermissionEnum.ReadWrite
                            ) ||
                            a.TicketCurrentStep == TicketCurrentStepEnum.Report
                          )
                      )
                    )
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == ticketId);

        if (ticket is not null && ticket.UserId == userId)
            return;

        if (ticket is null || ticket.TicketShares.Count == 0)
            throw new ForbiddenException(userId);
    }

    /// <inheritdoc cref="ITicketShareRepository.CheckIfUserHaveAccessToComponentStrictAsync" />
    public async Task CheckIfUserHaveAccessToComponentStrictAsync(
    Guid ticketId, Guid userId, TicketCurrentStepEnum currentStepEnum, SharePermissionEnum requireSharePermissionEnum)
    {
        var ticket = await _context.Tickets
            .Include(e => e.TicketShares
                .Where(a => a.TicketId == ticketId &&
                       a.SharedUserId == userId &&
                       (a.RevokedAt > DateTime.UtcNow || a.RevokedAt == null) &&
                       a.Permission >= requireSharePermissionEnum &&
                          (
                            a.TicketCurrentStep == null ||
                            a.TicketCurrentStep == currentStepEnum ||
                            (
                                requireSharePermissionEnum == SharePermissionEnum.Read &&
                                a.TicketCurrentStep == currentStepEnum + stepIncreaserNumber
                            ) ||
                            a.TicketCurrentStep == TicketCurrentStepEnum.Report
                          )
                      )
                    )
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == ticketId);

        if (ticket is not null && ticket.UserId == userId)
            return;

        if (ticket is null || ticket.TicketShares.Count == 0)
            throw new ForbiddenException(userId);
    }

    /// <inheritdoc cref="ITicketShareRepository.IfPermissionAlreadyGivenToUserAsync" />
    public async Task<bool> IfPermissionAlreadyGivenToUserAsync(Guid ticketId, Guid sharedUserId)
    {
        return await _context.TicketShares
            .AnyAsync(ts => ts.TicketId == ticketId && ts.SharedUserId == sharedUserId);
    }

    /// <inheritdoc cref="ITicketShareRepository.VerifyIfUserIsTicketOwnerAsync" />
    public async Task<bool> VerifyIfUserIsTicketOwnerAsync(Guid ownerId, Guid ticketId)
    {
        return await _context.Tickets
            .AnyAsync(e => e.UserId == ownerId && e.Id == ticketId);
    }

    /// <inheritdoc cref="ITicketShareRepository.CreateTicketShareAsync" />
    public async Task CreateTicketShareAsync(TicketShare ticketShare)
    {
        await _context.AddAsync(ticketShare);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketShareRepository.CreateManyTicketSharesAsync" />
    public async Task CreateManyTicketSharesAsync(List<TicketShare> ticketShares)
    {
        await _context.AddRangeAsync(ticketShares);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="ITicketShareRepository.GetTicketSharesAsync" />
    public async Task<PaginationResponseDto<TicketShare>> GetTicketSharesAsync(
        Guid ticketId, PaginationRequestDto paginationRequestDto)
    {
        return await _context.TicketShares
            .Where(e => e.TicketId == ticketId)
            .GetFinalResultAsync(paginationRequestDto);
    }

    /// <inheritdoc cref="ITicketShareRepository.UpdateTicketShareAsync" />
    public async Task UpdateTicketShareAsync(Guid ticketShareId, TicketShare ticketShare)
    {
        await _context.TicketShares.Where(a => a.Id == ticketShareId)
            .ExecuteUpdateAsync(a => a
                .SetProperty(ts => ts.Permission, ts => ticketShare.Permission)
                .SetProperty(ts => ts.RevokedAt, ts => ticketShare.RevokedAt));
    }

    /// <inheritdoc cref="ITicketShareRepository.DeleteTicketShareAsync" />
    public async Task DeleteTicketShareAsync(Guid ticketShareId)
    {
        var numberOfDeletedRows = await _context.TicketShares
            .Where(ts => ts.Id == ticketShareId)
            .ExecuteDeleteAsync();

        if (numberOfDeletedRows != 1)
            throw new InvalidOperationException("Failed to delete ticket share");
    }

    /// <inheritdoc cref="ITicketShareRepository.DeleteManyTicketShareAsync" />
    public async Task DeleteManyTicketShareAsync(List<Guid> ticketShareIds)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var numberOfDeletedRows = await _context.TicketShares
                .Where(ts => ticketShareIds.Contains(ts.Id))
                .ExecuteDeleteAsync();

            if (numberOfDeletedRows != ticketShareIds.Count)
                throw new Exception();

            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Failed to delete ticket share");
        }
    }

    /// <inheritdoc cref="ITicketShareRepository.GetTicketIdByTicketShareIdAsync" />
    public async Task<Guid> GetTicketIdByTicketShareIdAsync(Guid ticketShareId)
    {
        return await _context.TicketShares
            .Where(a => a.Id == ticketShareId)
            .Select(a => a.TicketId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc cref="ITicketShareRepository.GetTicketTitleByTicketIdAsync" />
    public async Task<string?> GetTicketTitleByTicketIdAsync(Guid ticketId)
    {
        return await _context.Tickets
            .Where(a => a.Id == ticketId)
            .Select(a => a.Title)
            .FirstOrDefaultAsync();
    }
}
