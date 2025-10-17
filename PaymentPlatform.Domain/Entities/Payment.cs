using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Domain.Entities
{
    public enum PaymentStatus { Pending = 0, Success = 1, Failed = 2 }
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }
        public Guid CardId { get; set; }

        public decimal Amount { get; set; }               // čuva se kao numeric(18,2)
        public string Currency { get; set; } = "EUR";     // "EUR","USD",...
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? ProviderReference { get; set; }    // id iz mock/provajdera
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Card Card { get; set; } = default!;
    }
}
