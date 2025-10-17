namespace PaymentPlatform.Api.Contracts.Cards
{
    public class AddCardRequest
    {
        public string CardNumber { get; set; } = default!;
        public string CardHolderName { get; set; } = default!;
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string Cvv { get; set; } = default!; // NE čuvamo, samo validacija/mock
    }
}
