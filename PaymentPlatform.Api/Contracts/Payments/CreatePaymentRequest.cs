namespace PaymentPlatform.Api.Contracts.Payments
{
    public class CreatePaymentRequest
    {
        public Guid CardId { get; set; }
        public string CardToken { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
    }
}
