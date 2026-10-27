namespace ClarityGuildWebsite.Api.Models
{
    public class BotDmMessage
    {
        public required string Sender { get; set; }
        public required string Recipient { get; set; }
        public string? MessageBody { get; set; }
        public bool Delivered { get; set; }
    }
}
