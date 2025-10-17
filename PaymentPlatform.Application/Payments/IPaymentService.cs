using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payments
{
    public record CreatePaymentCommand(Guid CardId, string CardToken, decimal Amount, string Currency);
    public record PaymentDto(Guid Id, Guid CardId, decimal Amount, string Currency, string Status, DateTime CreatedAt);
    public interface IPaymentService
    {
        Task<PaymentDto> CreateAsync(Guid userId, CreatePaymentCommand cmd, CancellationToken ct = default);
        Task<IReadOnlyList<PaymentDto>> HistoryAsync(Guid userId, PaymentHistoryFilter filter, CancellationToken ct = default);
    }
}
