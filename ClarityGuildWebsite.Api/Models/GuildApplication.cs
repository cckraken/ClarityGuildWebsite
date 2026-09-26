using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.Models
{
    public class GuildApplication
    {
        //form facing fields
        [MaxLength(50)]
        public required string DiscordId { get; set; }

        [MaxLength(50)]
        public required string MainName { get; set; }

        [MaxLength(50)]
        public required string MainRealm { get; set; }

        [MaxLength(50)]
        public required string MainRole { get; set; }

        [MaxLength(50)]
        public required string AltName { get; set; }

        [MaxLength(50)]
        public required string AltRealm { get; set; }

        [MaxLength(50)]
        public required string AltRole { get; set; }

        [MaxLength(100)]
        public required string Screenshot { get; set; }

        [MaxLength(500)]
        public required string Schedule { get; set; }

        public bool Splits { get; set; }
        public bool Goals { get; set; }

        [MaxLength(1500)]
        public required string About { get; set; }

        [MaxLength(1500)]
        public required string Tech { get; set; }

        [MaxLength(1500)]
        public required string History { get; set; }
        
        [MaxLength(1500)]
        public string? Extra {  get; set; }

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
