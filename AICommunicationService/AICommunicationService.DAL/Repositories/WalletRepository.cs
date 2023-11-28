using AICommunicationService.DAL.Context.Wallets;
using AICommunicationService.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AICommunicationService.DAL.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly WalletContext _context;
        public WalletRepository(WalletContext context)
        {
            _context = context;
        }

        /// <inheritdoc cref="IWalletRepository.TakeServiceFeeAsync(Guid, int)"/>
        public async Task TakeServiceFeeAsync(Guid userId, int feeAmount)
        {
            await _context.Wallets
                .Where(a => a.UserId == userId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(x => x.Balance, x => x.Balance - feeAmount));
        }

        /// <inheritdoc cref="IWalletRepository.CheckIfUserAllowedToCreateRequest(Guid, int)"/>
        public async Task<bool> CheckIfUserAllowedToCreateRequest(Guid userId, int userMinimum)
        {
            return await _context.Wallets.AnyAsync(a => a.UserId == userId && a.Balance <= userMinimum);
        }
    }
}
