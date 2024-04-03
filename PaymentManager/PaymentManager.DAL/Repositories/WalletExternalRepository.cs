using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Context;
using PaymentManager.DAL.Interfaces;

namespace PaymentManager.DAL.Repositories;
public class WalletExternalRepository : IWalletExternalRepository
{
    private readonly PaymentContext _context;
    public WalletExternalRepository(PaymentContext context)
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
