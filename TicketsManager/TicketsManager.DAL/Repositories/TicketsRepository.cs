using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;

namespace TicketsManager.DAL.Repositories;

public class TicketsRepository : ITicketsRepository
{
    private readonly TicketsManagerDbContext _context;

    public TicketsRepository(TicketsManagerDbContext context)
    {
        _context = context;
    }
}
