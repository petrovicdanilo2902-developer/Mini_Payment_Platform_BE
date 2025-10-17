using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Api.Contracts.Cards;
using PaymentPlatform.Domain.Entities;
using PaymentPlatform.Infrastructure.Data;
using System.Security.Claims;

namespace PaymentPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CardsController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Add(AddCardRequest req, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new("No user id"));

            // osn. validacija (mock)
            if (string.IsNullOrWhiteSpace(req.CardNumber) || req.CardNumber.Length < 12)
                return BadRequest("Invalid card number.");

            string last4 = new string(req.CardNumber.TakeLast(4).ToArray());
            string brand = req.CardNumber.StartsWith("4") ? "Visa"
                         : req.CardNumber.StartsWith("5") ? "Mastercard"
                         : "Card";

            var card = new Card
            {
                UserId = userId,
                Brand = brand,
                Last4 = last4,
                ExpirationMonth = req.ExpirationMonth,
                ExpirationYear = req.ExpirationYear,
                Token =
                brand.Equals("visa", StringComparison.OrdinalIgnoreCase)
                            ? "tok_visa"
                            : "tok_mastercard"
            };

            _db.Cards.Add(card);
            await _db.SaveChangesAsync(ct);

            return Ok(new { card.Id, card.Brand, card.Last4, card.ExpirationMonth, card.ExpirationYear });
        }

        [HttpGet]
        public async Task<IActionResult> MyCards(CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new("No user id"));

            var cards = await _db.Cards
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new { c.Id, c.Brand, c.Last4, c.ExpirationMonth, c.ExpirationYear })
                .ToListAsync(ct);

            return Ok(cards);
        }
    }
}
