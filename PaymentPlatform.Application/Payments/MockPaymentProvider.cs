using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaymentPlatform.Application.Payments.IPaymentProvider;

namespace PaymentPlatform.Application.Payments
{
    //public class MockPaymentProvider : IPaymentProvider
    //{
    //    private static readonly Random _rng = new();

    //    public async Task<ProviderChargeResult> ChargeAsync(ProviderChargeRequest req, CancellationToken ct = default)
    //    {
    //        await Task.Delay(1200, ct); // simulacija mreže

    //        // ~85% uspeha
    //        bool success = _rng.NextDouble() < 0.85;
    //        string reference = $"mock_{Guid.NewGuid():N}";

    //        return success
    //            ? new ProviderChargeResult(true, reference, null)
    //            : new ProviderChargeResult(false, reference, "Card declined (mock)");
    //    }
    //}

    using System;
    using System.Threading;
    using System.Threading.Tasks;

    namespace PaymentPlatform.Infrastructure.Services
    {
        public class MockPaymentProvider : IPaymentProvider
        {
            public Task<ProviderChargeResult> ChargeAsync(ProviderChargeRequest request, CancellationToken ct = default)
            {
                var result = new ProviderChargeResult(
                    true,
                    Guid.NewGuid().ToString(),
                    null
                );

                return Task.FromResult(result);
            }
        }
    }

}
