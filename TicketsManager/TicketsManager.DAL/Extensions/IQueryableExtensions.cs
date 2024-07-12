using TicketsManager.Common.Dtos;
using TicketsManager.Common.Helpers;
using TicketsManager.DAL.Dtos;
using TicketsManager.DAL.Models.TicketModels;
using TicketsManager.DAL.Models.TicketSummaryModels;
using TicketsManager.DAL.Models.UsecaseModels;

namespace TicketsManager.DAL.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginationResponseDto<ReturnUserStoryTestModel>> GetFinalResultAsync(
        this IQueryable<ReturnUserStoryTestModel> notification, PaginationRequestDto pagination)
    {
        return await notification
            .GetPaginatedCollectionAsync(pagination);
    }

    public static async Task<PaginationResponseDto<TicketSummary>> GetFinalResultAsync(
        this IQueryable<TicketSummary> notification, PaginationRequestDto pagination)
    {
        return await notification
            .GetPaginatedCollectionAsync(pagination);
    }

    public static async Task<PaginationResponseDto<Notification>> GetFinalResultAsync(
        this IQueryable<Notification> notification, PaginationRequestDto pagination)
    {
        return await notification
            .GetPaginatedCollectionAsync(pagination);
    }

    public static async Task<PaginationResponseDto<TicketShare>> GetFinalResultAsync(
        this IQueryable<TicketShare> ticketShares, PaginationRequestDto pagination)
    {
        return await ticketShares
            .GetPaginatedCollectionAsync(pagination);
    }

    public static async Task<PaginationResponseDto<Message>> GetFinalResultAsync(
        this IQueryable<Message> messages, PaginationRequestDto pagination)
    {
        return await messages
            .GetPaginatedCollectionAsync(pagination);
    }

    public static async Task<PaginationResponseDto<Ticket>> GetFinalResultAsync(
        this IQueryable<Ticket> tickets, PaginationRequestDto pagination)
    {
        return await tickets
            .OrderTicketsByDate()
            .GetPaginatedCollectionAsync(pagination);
    }

    public static async Task<PaginationResponseDto<ActionHistory>> GetFinalResultAsync(
        this IQueryable<ActionHistory> actionHistories, PaginationRequestDto pagination)
    {
        return await actionHistories
            .OrderHistoryByDate()
            .GetPaginatedCollectionAsync(pagination);
    }

    public static async Task<PaginationResponseDto<Usecase>> GetFinalResultAsync(
        this IQueryable<Usecase> usecases, PaginationRequestDto pagination)
    {
        return await usecases
            .GetPaginatedCollectionAsync(pagination);
    }

    private static IQueryable<Ticket> OrderTicketsByDate(
        this IQueryable<Ticket> tickets)
    {
        return tickets.OrderByDescending(e => e.UpdatedDate ?? e.CreatedDate);
    }

    private static IQueryable<ActionHistory> OrderHistoryByDate(
        this IQueryable<ActionHistory> actionHistories)
    {
        return actionHistories.OrderByDescending(e => e.ActionTime);
    }
}
