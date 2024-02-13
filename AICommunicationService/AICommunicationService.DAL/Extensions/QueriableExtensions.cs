using AICommunicationService.Common.Dtos;
using AICommunicationService.Common.Helpers;
using AICommunicationService.DAL.Models;

namespace AICommunicationService.DAL.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginationResponseDto<User>> GetUsersPaginatedResult(
        this IQueryable<User> users,
        PaginationRequestDto pagination)
    {
        return await users
            .GetPaginatedCollection(pagination);
    }
}