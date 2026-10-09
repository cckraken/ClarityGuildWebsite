using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.DTOs
{
    public class CreateApplicationDTO
    {
        [Required]
        [StringLength(50)]
        public string DiscordId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Country {  get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MainName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MainRealm { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MainRole { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AltName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AltRealm { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string AltRole { get; set; } = string.Empty;     

        [Required]
        [StringLength(100)]
        public string Screenshot { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string About { get; set; } = string.Empty;

        [StringLength(500)]
        [Required]
        public string Tech { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string History { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Extra { get; set; }
    }
}
