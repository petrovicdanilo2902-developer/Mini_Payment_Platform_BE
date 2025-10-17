using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPlatform.Domain.Entities
{
    public class Card
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Pretpostavljam da je User.Id = int. Ako je Guid kod tebe, promeni tip.
        public Guid UserId { get; set; }

        public string Brand { get; set; } = default!;     // npr. "Visa", "Mastercard"
        public string Last4 { get; set; } = default!;     // "4242"
        public int ExpirationMonth { get; set; }          // 1-12
        public int ExpirationYear { get; set; }           // npr. 2027

        // Token koji bi dobili od pravog procesora (ovde mock vrednost)
        public string Token { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
