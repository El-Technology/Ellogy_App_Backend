using Microsoft.EntityFrameworkCore;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Exceptions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories;

public class UserRepository : IUserRepository
{
	private readonly TicketsManagerDbContext _context;

	public UserRepository(TicketsManagerDbContext context)
	{
		_context = context;
	}

	public async Task<User> GetUserAsync(Guid id)
	{
		var user = await _context.Users
					   .AsNoTracking()
					   .Include(e => e.UserTickets)
					   .ThenInclude(e => e.TicketMessages)
					   .FirstOrDefaultAsync(e => e.Id == id)
				   ?? throw new EntityNotFoundException(typeof(User));

		foreach (var ticket in user.UserTickets)
			ticket.TicketMessages = ticket.TicketMessages.OrderBy(e => e.SendTime).ToList();

		return user;
	}

	public Task<bool> CheckIfUserExistAsync(Guid id)
	{
		return _context.Users.AnyAsync(e => e.Id == id);
	}
}
