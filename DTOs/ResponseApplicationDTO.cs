namespace ClarityGuildWebsite.Api.DTOs
{

    public class ApplicantResponseDTO
    {
        public string DiscordId { get; set; } = string.Empty;
        public int? Age { get; set; }
        public string Country { get; set; } = string.Empty;
        public string WarcraftLogsLink { get; set; } = string.Empty;
        public string Tech { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public bool Splits { get; set; }
        public string Communication { get; set; } = string.Empty;
        public string? History { get; set; }
        public string Screenshot { get; set; } = string.Empty;
        public string? Vouch { get; set; }
        public bool? Goals { get; set; }
    }

    public class OfficerResponseDTO : ApplicantResponseDTO
    {
        public int Id { get; set; }
        public ApplicationStatus Status { get; set; }
        public PostStatus PostStatus { get; set; }
        public DateTime TimestampUtc { get; set; }
    }

    public class AdministratorResponseDTO : OfficerResponseDTO
    {
        public string? DiscordMessageId { get; set; }
        public DateTime? LastAttemptedUtc { get; set; }
        public int Retry { get; set; }
    }
}

