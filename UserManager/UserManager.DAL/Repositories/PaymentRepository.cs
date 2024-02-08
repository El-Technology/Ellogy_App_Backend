using UserManager.DAL.Context.PaymentContext;
using UserManager.DAL.Interfaces;
using UserManager.DAL.Models;

namespace UserManager.DAL.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentContext _context;
        public PaymentRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task CreateWalletForNewUserAsync(Guid userId)
        {
            await _context.Wallets.AddAsync(new Wallet { Balance = 0, Id = Guid.NewGuid(), UserId = userId });
            await _context.SaveChangesAsync();
        }
    }
}
