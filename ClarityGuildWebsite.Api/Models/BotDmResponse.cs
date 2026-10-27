namespace ClarityGuildWebsite.Api.Models
{
    public class BotDmResponse
    {
        public required string ApplicantDiscordId { get; set; }
        public required string ApplicantDiscordName { get; set; }
        public string? MessageBody { get; set; }
        public bool Delivered { get; set; }
    }
}
