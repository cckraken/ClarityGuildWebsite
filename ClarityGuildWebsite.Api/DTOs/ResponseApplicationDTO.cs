namespace ClarityGuildWebsite.Api.DTOs
{

    public class ApplicantResponseDTO
    {
        //Guid to be added once endpoint exposed on live site
        public DateTime TimestampUtc { get; set; }

    }

    public class OfficerResponseDTO : ApplicantResponseDTO
    {
        public string DiscordId { get; set; } = string.Empty;
        public string DiscordName { get; set; } = string.Empty;
        public string Country {  get; set; } = string.Empty;
        public int? Age { get; set; }
        public string MainName { get; set; } = string.Empty;
        public string MainRealm { get; set; } = string.Empty;
        public string MainRole { get; set; } = string.Empty;
        public string AltName { get; set; } = string.Empty;
        public string AltRealm { get; set; } = string.Empty;
        public string AltRole { get; set; } = string.Empty;
        public string? Preparation { get; set; }
        public string About { get; set; } = string.Empty;
        public string Tech { get; set; } = string.Empty;
        public string History { get; set; } = string.Empty;
        public string? Extra { get; set; }
        //PublicId and Id to be added later
        public ApplicationStatus Status { get; set; }
        public PostStatus PostStatus { get; set; }
        public string? LastPostError {  get; set; } = string.Empty;
    }

    public class AdministratorResponseDTO : OfficerResponseDTO
    {
        public string? DiscordMessageId { get; set; }
        public DateTime? LastAttemptedUtc { get; set; }
        public int Retry { get; set; }
    }
}

