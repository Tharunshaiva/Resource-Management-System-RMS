using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.FlaskDTO
{
    public class WorkingDaysResponseDTO
    {
        //[JsonPropertyName("success")]
        //public bool Success { get; set; }

        //[JsonPropertyName("working_days")]
        //public List<ProjectWorkingDaysDTO> WorkingDays { get; set; }

        [JsonPropertyName("success")]
        [Required(ErrorMessage = "Success flag is required.")]
        public bool Success { get; set; }

        [JsonPropertyName("working_days")]
        [Required(ErrorMessage = "WorkingDays list is required.")]
        [MinLength(1, ErrorMessage = "WorkingDays must contain at least one item.")]
        public List<ProjectWorkingDaysDTO> WorkingDays { get; set; }
    }
}
