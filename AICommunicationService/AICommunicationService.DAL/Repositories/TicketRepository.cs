using AICommunicationService.DAL.Context.Tickets;
using AICommunicationService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.DAL.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly TicketContext _context;
        public TicketRepository(TicketContext context)
        {
            _context = context;
        }

        public async Task<int?> UpdateTokensUsageAsync(Guid ticketId, int usedTokens)
        {
            var result = await _context.Tickets.Where(a => a.Id == ticketId).ExecuteUpdateAsync(x => x.SetProperty(x => x.TokensTotalUsage, x => x.TokensTotalUsage + usedTokens));
            if (result == 0 && ticketId != new Guid())
                throw new Exception($"Ticket with id - {ticketId} was not found");

            return (await _context.Tickets.FirstOrDefaultAsync(a => a.Id == ticketId))?.TokensTotalUsage;
        }
    }
}
