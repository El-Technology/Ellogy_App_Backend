using Microsoft.EntityFrameworkCore;
using TicketsManager.Common.Dtos;
using TicketsManager.DAL.Context;
using TicketsManager.DAL.Interfaces;
using TicketsManager.DAL.Models;

namespace TicketsManager.DAL.Repositories
{
    public class UsecaseRepository : IUsecaseRepository
    {
        private const int OneTableToOneTicket = 1;
        private const int PaginationStartsFromOne = 1;
        private readonly TicketsManagerDbContext _context;
        public UsecaseRepository(TicketsManagerDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IUsecaseRepository.CreateUsecasesAsync(TicketTable, List{TicketDiagram})"/>
        public async Task CreateUsecasesAsync(TicketTable table, List<TicketDiagram> diagrams)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.AddAsync(table);
                await _context.AddRangeAsync(diagrams);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }
        }

        /// <inheritdoc cref="IUsecaseRepository.GetTableAsync(Guid)"/>
        public async Task<TicketTable?> GetTableAsync(Guid ticketId)
        {
            var tables = await _context.TicketTables.Where(t => t.TicketId == ticketId).ToListAsync();
            if (tables.Count > OneTableToOneTicket)
            {
                _context.TicketTables.RemoveRange(tables.SkipLast(OneTableToOneTicket));
                await _context.SaveChangesAsync();
            }
            return await _context.TicketTables.Include(a => a.TableValues).FirstOrDefaultAsync(a => a.TicketId == ticketId);
        }

        /// <inheritdoc cref="IUsecaseRepository.GetDiagramsAsync(PaginationRequestDto, Guid)"/>
        public async Task<PaginationResponseDto<TicketDiagram>> GetDiagramsAsync(PaginationRequestDto paginationRequest, Guid ticketId)
        {
            var numberOfItemsToSkip = (paginationRequest.CurrentPageNumber - PaginationStartsFromOne) * paginationRequest.RecordsPerPage;
            var totalRecords = _context.TicketDiagrams.Where(a => a.TicketId == ticketId).Count();

            var ticketDiagrams = await _context.TicketDiagrams
                .Where(a => a.TicketId == ticketId)
                .Skip(numberOfItemsToSkip)
                .Take(paginationRequest.RecordsPerPage)
                .ToListAsync();

            var response = new PaginationResponseDto<TicketDiagram>()
            {
                Data = ticketDiagrams,
                CurrentPageNumber = paginationRequest.CurrentPageNumber,
                RecordsPerPage = paginationRequest.RecordsPerPage,
                TotalRecordsFound = totalRecords,
                RecordsReturned = ticketDiagrams.Count
            };
            return response;
        }

        /// <inheritdoc cref="IUsecaseRepository.UpdateTableAsync(TicketTable)"/>
        public async Task UpdateTableAsync(TicketTable ticketTable)
        {
            _context.Update(ticketTable);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc cref="IUsecaseRepository.UpdateDiagramAsync(TicketDiagram)"/>
        public async Task UpdateDiagramAsync(TicketDiagram ticketDiagram)
        {
            _context.Update(ticketDiagram);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc cref="IUsecaseRepository.GetTableByIdAsync(Guid)"/>
        public Task<TicketTable?> GetTableByIdAsync(Guid tableId)
        {
            return _context.TicketTables
                           .Include(a => a.TableValues)
                           .AsTracking()
                           .FirstOrDefaultAsync(a => a.Id == tableId);
        }

        /// <inheritdoc cref="IUsecaseRepository.GetDiagramByIdAsync(Guid)"/>
        public Task<TicketDiagram?> GetDiagramByIdAsync(Guid diagramId)
        {
            return _context.TicketDiagrams
                           .AsTracking()
                           .FirstOrDefaultAsync(a => a.Id == diagramId);
        }
    }
}
