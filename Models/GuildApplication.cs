namespace ClarityGuildWebsite.Api.Models
{
    public class ApplicationForm
    {
        //form facing fields
        public required string DiscordId { get; set; }
        public int? Age { get; set; }
        public required string Country { get; set; }
        public required string WarcraftLogsLink { get; set; }
        public required string Tech {  get; set; }
        public required string Schedule {  get; set; }
        public bool? Splits { get; set; }
        public required string Communication { get; set; }
        public required string Screenshot {  get; set; }
        public string? Vouch {  get; set; }
        public bool? Goals { get; set; }

        //db only
        public int ApplicationId { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string? DiscordMessageId { get; set; }
        public DateTime? LastAttemptedUtc { get; set; }
        public int Retry {  get; set; }
        public ApplicationStatus Status { get; set; }
        public PostStatus PostStatus { get; set; }
    }
}
