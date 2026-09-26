namespace ClarityGuildWebsite.Api.DTOs
{

    public class ApplicantResponseDTO
    {
        public string DiscordId { get; set; } = string.Empty;
        public string MainName {  get; set; } = string.Empty;
        public string MainRealm { get; set;  } = string.Empty;
        public string MainRole {  get; set; } = string.Empty;
        public string AltName { get; set; } = string.Empty;
        public string AltRealm { get; set; } = string.Empty;
        public string AltRole { get; set; } = string.Empty;
        public string Screenshot { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public bool Splits { get; set; }
        public bool Goals { get; set; }
        public string About {  get; set; } = string.Empty;
        public string Tech { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;
        public string? Extra { get; set; }
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

