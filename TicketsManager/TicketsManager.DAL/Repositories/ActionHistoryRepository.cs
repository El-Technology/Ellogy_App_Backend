using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Enums;
using TicketsManager.DAL.Extensions;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories
{
    public class ActionHistoryRepository : IActionHistoryRepository
    {
        private readonly TicketsManagerDbContext _context;
        public ActionHistoryRepository(TicketsManagerDbContext context)
        {
            _context = context;
        }

        public async Task CreateActionHistoryAsync(ActionHistory actionHistory)
        {
            await _context.ActionHistories.AddAsync(actionHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginationResponseDto<ActionHistory>> GetActionHistoriesAsync(Guid ticketId,
                                                                                        TicketCurrentStepEnum ticketCurrentStepEnum,
                                                                                        PaginationRequestDto paginationRequest)
        {
            var histories = await _context.ActionHistories.Where(a => a.TicketId == ticketId).ToListAsync();

            return histories
                .Where(a => a.TicketCurrentStepEnum == ticketCurrentStepEnum)
                .GetFinalResult(paginationRequest);
        }
    }
}
