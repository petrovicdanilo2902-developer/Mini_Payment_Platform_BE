using PaymentPlatform.Application.Payments;
using PaymentPlatform.Domain.Entities;
using PaymentPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static PaymentPlatform.Application.Payments.IPaymentProvider;


namespace PaymentPlatform.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _db;
        private readonly IPaymentProvider _provider;

        public PaymentService(AppDbContext db, IPaymentProvider provider)
        {
            _db = db;
            _provider = provider;
        }

        public async Task<PaymentDto> CreateAsync(Guid userId, CreatePaymentCommand cmd, CancellationToken ct = default)
        {
            var card = await _db.Cards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == cmd.CardId && c.UserId == userId, ct);

            if (card is null)
                throw new InvalidOperationException("Card not found for this user.");

            var payment = new Payment
            {
                UserId = userId,
                CardId = card.Id,
                Amount = cmd.Amount,
                Currency = cmd.Currency,
                Status = PaymentStatus.Pending
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync(ct); // upiši Pending

            // Pozovi "procesor"
            var result = await _provider.ChargeAsync(
                new ProviderChargeRequest(cmd.Amount, cmd.Currency, card.Token), ct);

            payment.Status = result.Success ? PaymentStatus.Success : PaymentStatus.Failed;
            payment.ProviderReference = result.Reference;
            await _db.SaveChangesAsync(ct);

            return new PaymentDto(payment.Id, payment.CardId, payment.Amount, payment.Currency,
                                  payment.Status.ToString(), payment.CreatedAt);
        }

        public async Task<IReadOnlyList<PaymentDto>> HistoryAsync(Guid userId, CancellationToken ct = default)
        {
            return await _db.Payments
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentDto(p.Id, p.CardId, p.Amount, p.Currency, p.Status.ToString(), p.CreatedAt))
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<PaymentDto>> HistoryAsync(
    Guid userId, PaymentHistoryFilter filter, CancellationToken ct = default)
        {
            var query = _db.Payments
                .AsNoTracking()
                .Where(p => p.UserId == userId);

            if (!string.IsNullOrEmpty(filter.Status))
            {
                if (Enum.TryParse<PaymentStatus>(filter.Status, true, out var parsedStatus))
                    query = query.Where(p => p.Status == parsedStatus);
            }

            query = query.OrderByDescending(p => p.CreatedAt);

            return await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(p => new PaymentDto(
                    p.Id, p.CardId, p.Amount, p.Currency, p.Status.ToString(), p.CreatedAt))
                .ToListAsync(ct);
        }

    }
}
