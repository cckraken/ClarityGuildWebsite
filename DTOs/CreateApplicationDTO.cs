using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.DTOs
{
    public class CreateApplicationDTO
    {
        [Required]
        [StringLength(50)]
        public  string DiscordId { get; set; } = string.Empty;
        public int? Age { get; set; }
        [Required]
        [StringLength(30)]
        public string Country { get; set; } = string.Empty;
        [Required]
        [Url]
        [StringLength(500)]
        public string WarcraftLogsLink { get; set; } = string.Empty;
        [StringLength(500)]
        [Required]
        public string Tech { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Schedule { get; set; } = string.Empty;
        [Required]
        public bool? Splits { get; set; }

        [Required]
        [StringLength(150)]
        public string Communication { get; set; } = string.Empty;
        [StringLength(2000)]
        public string? History { get; set; }
        [Required]
        [Url]
        [StringLength(100)]
        public string Screenshot { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Vouch { get; set; }
        public bool? Goals { get; set; }
    }
}
