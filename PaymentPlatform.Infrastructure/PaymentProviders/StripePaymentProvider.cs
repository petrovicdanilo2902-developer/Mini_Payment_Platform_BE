using Microsoft.Extensions.Configuration;
using PaymentPlatform.Application.Payments;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Infrastructure.PaymentProviders
{
    public class StripePaymentProvider : IPaymentProvider
    {
        private readonly string _apiKey;
        private readonly string _publicKey;

        public StripePaymentProvider(IConfiguration configuration)
        {
            _apiKey = configuration["Stripe:SecretKey"]
                ?? throw new ArgumentNullException("Stripe SecretKey missing in configuration.");

            _publicKey = configuration["Stripe:PublishableKey"]
               ?? throw new ArgumentNullException("Stripe PublishableKey missing in configuration.");
        }

        public async Task<ProviderChargeResult> ChargeAsync(ProviderChargeRequest req, CancellationToken ct = default)
        {
            StripeConfiguration.ApiKey = _apiKey;

            var options = new ChargeCreateOptions
            {
                Amount = (long)(req.Amount * 100), // u centima
                Currency = req.Currency.ToLower(),
                Description = "Mini Payment Platform test charge",
                Source = req.CardToken, // ovde koristimo token (npr. 'tok_visa')
            };

            var service = new ChargeService();
            try
            {
                var charge = await service.CreateAsync(options, cancellationToken: ct);
                return new ProviderChargeResult(true, charge.Id, null);
            }
            catch (StripeException ex)
            {
                return new ProviderChargeResult(false, string.Empty, ex.Message);
            }
        }
    }
}
