using Microsoft.EntityFrameworkCore;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Context.PaymentContext;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentContext _context;

        public PaymentRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task<Wallet> CreateUserWalletAsync(Guid userId)
        {
            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Balance = Constants.NewWalletBalance
            };

            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task<Wallet?> GetUserWalletAsync(Guid userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(a => a.UserId == userId);
        }

        public async Task UpdateBalanceAsync(Guid userId, int amountOfPoints)
        {
            await _context.Wallets
                .Where(a => a.UserId == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.Balance, x => x.Balance + amountOfPoints));
        }

        public async Task<Payment?> GetPaymentByIdAsync(string paymentId)
        {
            return await _context.Payments.FirstOrDefaultAsync(a => a.PaymentId.Equals(paymentId));
        }

        public async Task UpdatePaymentAsync(Payment payment)
        {
            await _context.Payments
                .Where(a => a.SessionId.Equals(payment.SessionId))
                .ExecuteUpdateAsync(u => u
                .SetProperty(s => s.Status, s => payment.Status)
                .SetProperty(p => p.PaymentId, s => payment.PaymentId)
                .SetProperty(p => p.UpdatedBallance, s => payment.UpdatedBallance));
        }

        public async Task CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetPaymentAsync(string sessionId)
        {
            return await _context.Payments.FirstOrDefaultAsync(a => a.SessionId.Equals(sessionId));
        }
    }
}
