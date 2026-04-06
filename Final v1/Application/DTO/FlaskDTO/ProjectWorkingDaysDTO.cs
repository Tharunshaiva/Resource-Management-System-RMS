using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.FlaskDTO
{
    public class ProjectWorkingDaysDTO
    {
        //[JsonPropertyName("projectID")]
        //public string ProjectID { get; set; }

        //[JsonPropertyName("projectName")]
        //public string ProjectName { get; set; }

        //[JsonPropertyName("workingDays")]
        //public int WorkingDays { get; set; }

        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [RegularExpression(@"\S+", ErrorMessage = "ProjectID cannot be empty or whitespace.")]
        [StringLength(20, ErrorMessage = "ProjectID cannot exceed 20 characters.")]
        public string ProjectID { get; set; }

        [JsonPropertyName("projectName")]
        [Required(ErrorMessage = "ProjectName is required.")]
        [RegularExpression(@"\S+", ErrorMessage = "ProjectName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string ProjectName { get; set; }

        [JsonPropertyName("workingDays")]
        [Range(1, 365, ErrorMessage = "WorkingDays must be between 1 and 365.")]
        public int WorkingDays { get; set; }
    }
}
