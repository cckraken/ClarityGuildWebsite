using System.ComponentModel.DataAnnotations;

namespace ClarityGuildWebsite.Api.DTOs
{
    public class CreateApplicationDTO
    {
        [Required]
        [StringLength(50)]
        public string Country {  get; set; } = string.Empty;

        [Required]
        [Range(13, 99)]
        public int? Age { get; set; }

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
        [StringLength(1024)]
        public string? Preparation { get; set; } = string.Empty;

        [Required]
        [StringLength(1024)]
        public string About { get; set; } = string.Empty;

        [StringLength(500)]
        [Required]
        public string Tech { get; set; } = string.Empty;

        [Required]
        [StringLength(1024)]
        public string History { get; set; } = string.Empty;

        [StringLength(1024)]
        public string? Extra { get; set; }
    }
}
