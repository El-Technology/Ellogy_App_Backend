using UserManager.Common.Dtos;
using UserManager.Common.Helpers;
using UserManager.DAL.Models;

namespace UserManager.DAL.Extensions;
public static class QueryableExtensions
{
    public static async Task<PaginationResponseDto<User>> GetUsersPaginatedResult(
        this IQueryable<User> users,
        PaginationRequestDto pagination)
    {
        return await users
            .GetPaginatedCollectionAsync(pagination);
    }
}
