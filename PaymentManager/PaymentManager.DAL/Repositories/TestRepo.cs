using Microsoft.EntityFrameworkCore;
using PaymentManager.DAL.Context;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Repositories
{
    public class TestRepo
    {
        private readonly PaymentContext _context;
        public TestRepo(PaymentContext context)
        {
            _context = context;
        }

        public async Task<Wallet> CreateUserWalletAsync(string userEmail)
        {
            var wallet = new Wallet
            {
                UserEmail = userEmail,
                Balance = 0
            };

            await _context.Walets.AddAsync(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task<Wallet?> GetUserWalletAsync(string userEmail)
        {
            return await _context.Walets.FirstOrDefaultAsync(a => a.UserEmail.Equals(userEmail));
        }

        public async Task UpdateBalance(string email, Guid productId)
        {
            var addToBalance = int.Parse((await _context.Products.FirstOrDefaultAsync(a => a.Id == productId))?.Title);

            await _context.Walets
                .Where(a => a.UserEmail.Equals(email))
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.Balance, x => x.Balance + addToBalance));
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _context.Products.FindAsync(productId);
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
