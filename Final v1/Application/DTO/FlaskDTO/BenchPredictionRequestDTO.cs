using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.FlaskDTO
{
    public class BenchPredictionRequestDTO
    {
        //[JsonPropertyName("skill")]
        //public string Skill { get; set; }

        //[JsonPropertyName("experience")]
        //public double Experience { get; set; }

        [JsonPropertyName("skill")]
        [Required(ErrorMessage = "Skill is required.")]
        [StringLength(50, ErrorMessage = "Skill name cannot exceed 50 characters.")]
        public string Skill { get; set; }

        [JsonPropertyName("experience")]
        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public double Experience { get; set; }
    }
}
