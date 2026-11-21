using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.Models.Applications
{
    [Index(nameof(PublicId), IsUnique = true)]
    [Index(nameof(DiscordThreadId), IsUnique = true)]
    public class GuildApplication
    {
        //form facing fields
        public required string DiscordId { get; set; }
        public required string DiscordName { get; set; }
        public required string Country { get; set; }
        [Range(18, 99)]
        public int? Age { get; set; }
        public required string MainName { get; set; }
        public required string MainRealm { get; set; }
        public required string MainRole { get; set; }
        public required string AltName { get; set; }
        public required string AltRealm { get; set; }
        public required string AltRole { get; set; }
        public string? Preparation { get; set; }
        public required string About { get; set; }
        public required string Tech { get; set; }
        public required string History { get; set; }
        public string? Extra { get; set; }

        //db only
        public Guid PublicId { get; set; }
        public int Id { get; set; }
        public DateTime TimestampUtc { get; set; }
        public string? DiscordMessageId { get; set; }
        public string? DiscordThreadId { get; set; }
        public string? LastPostError { get; set; }
        public DateTime? LastAttemptedUtc { get; set; }
        public DateTime? LastRecruitmentTeamMessagedUtc { get; set; }
        public int Retry { get; set; }
        public ApplicationStatus Status { get; set; }
        public PostStatus PostStatus { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
