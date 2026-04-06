using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.RrportDTO
{
    public class SkillAvailabilityDTO
    {
        //public string SkillName { get; set; }
        //public int AvailableCount { get; set; }

        [JsonPropertyName("skillName")]
        [Required(ErrorMessage = "SkillName is required.")]
        [RegularExpression(@"\S+", ErrorMessage = "SkillName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "SkillName cannot exceed 100 characters.")]
        public string SkillName { get; set; }

        [JsonPropertyName("availableCount")]
        [Required(ErrorMessage = "AvailableCount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "AvailableCount cannot be negative.")]
        public int AvailableCount { get; set; }
    }
}
