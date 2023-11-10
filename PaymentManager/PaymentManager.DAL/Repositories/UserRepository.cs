using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Context.UserContext;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _userContext;
        public UserRepository(UserContext userContext)
        {
            _userContext = userContext;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userContext.Users.FindAsync(userId);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _userContext.Users.ToListAsync();
            return users;
        }
    }
}
