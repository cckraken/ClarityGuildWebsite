using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.Models
{
    public class GuildApplication
    {
        //form facing fields
        [MaxLength(50)]
        public required string DiscordId { get; set; }
        public int? Age { get; set; }
        [MaxLength(30)]
        public required string Country { get; set; }
        [MaxLength(500)]
        public required string WarcraftLogsLink { get; set; }
        [MaxLength(500)]
        public required string Tech {  get; set; }
        [MaxLength(500)]
        public required string Schedule {  get; set; }
        public bool Splits { get; set; }
        [MaxLength(150)]
        public required string Communication { get; set; }
        [MaxLength(2000)]
        public string? History { get; set; }
        [MaxLength(100)]
        public required string Screenshot {  get; set; }
        [MaxLength(100)]
        public string? Vouch {  get; set; }
        public bool? Goals { get; set; }

        //db only
        public int Id { get; set; }
        public DateTime TimestampUtc { get; set; }
        [MaxLength(50)]
        public string? DiscordMessageId { get; set; }
        public DateTime? LastAttemptedUtc { get; set; }
        public int Retry {  get; set; }
        public ApplicationStatus Status { get; set; }
        public PostStatus PostStatus { get; set; }
    }
}
