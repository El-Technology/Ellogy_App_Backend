using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories
{
    public class UsecaseRepository : IUsecaseRepository
    {
        private const int PaginationStartsFromOne = 1;
        private readonly TicketsManagerDbContext _context;
        public UsecaseRepository(TicketsManagerDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> GetUserIdByTicketIdAsync(Guid ticketId)
        {
            return await _context.Tickets.Where(a => a.Id == ticketId).Select(a => a.UserId).FirstOrDefaultAsync();
        }

        ///<inheritdoc cref="IUsecaseRepository.CreateUsecasesAsync(List{Usecase})"/>
        public async Task CreateUsecasesAsync(List<Usecase> usecases)
        {
            await _context.AddRangeAsync(usecases);
            await _context.SaveChangesAsync();
        }

        ///<inheritdoc cref="IUsecaseRepository.GetUsecasesAsync(PaginationRequestDto, Guid)"/>
        public async Task<PaginationResponseDto<Usecase>> GetUsecasesAsync(PaginationRequestDto paginationRequest, Guid ticketId)
        {
            var numberOfItemsToSkip = (paginationRequest.CurrentPageNumber - PaginationStartsFromOne) * paginationRequest.RecordsPerPage;
            var totalRecords = _context.Usecases.Where(a => a.TicketId == ticketId).Count();

            var usecases = await _context.Usecases
                .Where(a => a.TicketId == ticketId)
                .Skip(numberOfItemsToSkip)
                .Take(paginationRequest.RecordsPerPage)
                .Include(a => a.Tables)
                .Include(a => a.Diagrams)
                .ToListAsync();

            var response = new PaginationResponseDto<Usecase>()
            {
                Data = usecases,
                CurrentPageNumber = paginationRequest.CurrentPageNumber,
                RecordsPerPage = paginationRequest.RecordsPerPage,
                TotalRecordsFound = totalRecords,
                RecordsReturned = usecases.Count
            };
            return response;
        }

        ///<inheritdoc cref="IUsecaseRepository.UpdateUsecaseAsync(Usecase)"/>
        public async Task UpdateUsecaseAsync(Usecase usecase)
        {
            _context.Update(usecase);
            await _context.SaveChangesAsync();
        }

        ///<inheritdoc cref="IUsecaseRepository.GetUsecaseByIdAsync(Guid)"/>
        public Task<Usecase?> GetUsecaseByIdAsync(Guid usecaseId)
        {
            return _context.Usecases
                           .AsTracking()
                           .FirstOrDefaultAsync(a => a.Id == usecaseId);
        }

        /// <inheritdoc cref="IUsecaseRepository.DeleteUsecasesAsync(Guid)"/>
        public async Task DeleteUsecasesAsync(Guid ticketId)
        {
            var usecasesForDelete = await _context.Usecases.Where(a => a.TicketId == ticketId).ToListAsync();
            _context.Usecases.RemoveRange(usecasesForDelete);
            await _context.SaveChangesAsync();
        }
    }
}
