using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentPlatform.Api.Contracts.Payments;
using PaymentPlatform.Application.Payments;
using System.Security.Claims;

namespace PaymentPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _payments;

        public PaymentsController(IPaymentService payments) => _payments = payments;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequest req, CancellationToken ct)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new("No user id"));

            var dto = await _payments.CreateAsync(userId,
                new CreatePaymentCommand(req.CardId, req.CardToken, req.Amount, req.Currency), ct);

            return Ok(dto);
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(
    [FromQuery] string? status,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    CancellationToken ct = default)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new("No user id"));
            var filter = new PaymentHistoryFilter
            {
                Status = status,
                Page = page,
                PageSize = pageSize
            };

            var list = await _payments.HistoryAsync(userId, filter, ct);

            return Ok(list);
        }

    }
}
