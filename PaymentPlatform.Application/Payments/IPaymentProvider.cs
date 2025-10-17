//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PaymentPlatform.Application.Payments
//{
//    public interface IPaymentProvider
//    {
//        public record ProviderChargeRequest(decimal Amount, string Currency, string CardToken);
//        public record ProviderChargeResult(bool Success, string Reference, string? FailureReason);

//        public interface IPaymentProvider
//        {
//            Task<ProviderChargeResult> ChargeAsync(ProviderChargeRequest req, CancellationToken ct = default);
//        }
//    }
//}

using System.Threading;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payments
{
    // 🔹 Interfejs za plaćanje
    public interface IPaymentProvider
    {
        Task<ProviderChargeResult> ChargeAsync(ProviderChargeRequest req, CancellationToken ct = default);
    }

    // 🔹 DTO objekti (mogu biti u istom fajlu)
    public record ProviderChargeRequest(decimal Amount, string Currency, string CardToken);

    public record ProviderChargeResult(bool Success, string Reference, string? FailureReason);
}
