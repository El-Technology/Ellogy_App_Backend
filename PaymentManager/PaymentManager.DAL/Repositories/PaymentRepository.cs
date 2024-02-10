using Microsoft.EntityFrameworkCore;
using PaymentManager.Common.Constants;
using PaymentManager.DAL.Context.PaymentContext;
using PaymentManager.DAL.Interfaces;
using PaymentManager.DAL.Models;

namespace PaymentManager.DAL.Repositories;

/// <summary>
///     Repository for payment operations.
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly PaymentContext _context;

    public PaymentRepository(PaymentContext context)
    {
        _context = context;
    }

    /// <inheritdoc cref="IPaymentRepository.CreateUserWalletAsync(Guid)" />
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

    /// <inheritdoc cref="IPaymentRepository.GetUserWalletAsync(Guid)" />
    public async Task<Wallet?> GetUserWalletAsync(Guid userId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(a => a.UserId == userId);
    }

    /// <inheritdoc cref="IPaymentRepository.UpdateBalanceAsync(Guid, int)" />
    public async Task UpdateBalanceAsync(Guid userId, int amountOfPoints)
    {
        await _context.Wallets
            .Where(a => a.UserId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.Balance, x => x.Balance + amountOfPoints));
    }

    /// <inheritdoc cref="IPaymentRepository.GetPaymentByIdAsync(string)" />
    public async Task<Payment?> GetPaymentByIdAsync(string paymentId)
    {
        return await _context.Payments.FirstOrDefaultAsync(a => a.PaymentId.Equals(paymentId));
    }

    /// <inheritdoc cref="IPaymentRepository.UpdatePaymentAsync(Payment)" />
    public async Task UpdatePaymentAsync(Payment payment)
    {
        await _context.Payments
            .Where(a => a.SessionId.Equals(payment.SessionId))
            .ExecuteUpdateAsync(u => u
                .SetProperty(s => s.Status, s => payment.Status)
                .SetProperty(p => p.PaymentId, s => payment.PaymentId)
                .SetProperty(p => p.InvoiceId, p => payment.InvoiceId)
                .SetProperty(p => p.UpdatedBallance, s => payment.UpdatedBallance));
    }

    /// <inheritdoc cref="IPaymentRepository.CreatePaymentAsync(Payment)" />
    public async Task CreatePaymentAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc cref="IPaymentRepository.GetPaymentAsync(string)" />
    public async Task<Payment?> GetPaymentAsync(string sessionId)
    {
        return await _context.Payments.FirstOrDefaultAsync(a => a.SessionId.Equals(sessionId));
    }

    /// <inheritdoc cref="IPaymentRepository.GetPaymentByInvoiceIdAsync(string)" />
    public async Task<Payment?> GetPaymentByInvoiceIdAsync(string invoiceId)
    {
        return await _context.Payments.FirstOrDefaultAsync(a => a.InvoiceId.Equals(invoiceId));
    }

    /// <inheritdoc cref="IPaymentRepository.GetPaymentByInvoiceOrPaymentIdAsync(string, string)" />
    public async Task<Payment?> GetPaymentByInvoiceOrPaymentIdAsync(string paymentId, string invoiceId)
    {
        return await _context.Payments.FirstOrDefaultAsync(a =>
            a.PaymentId.Equals(paymentId) || a.InvoiceId.Equals(invoiceId));
    }
}