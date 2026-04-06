using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.RrportDTO
{
    public class ProjectOverlapDto
    {
        //public int EmployeeId { get; set; }
        //public string ProjectName { get; set; }
        //public int OverlapDays { get; set; }

        [JsonPropertyName("employeeId")]
        [Required(ErrorMessage = "EmployeeId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeId must be greater than 0.")]
        public int EmployeeId { get; set; }

        [JsonPropertyName("projectName")]
        [Required(ErrorMessage = "ProjectName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]

        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string ProjectName { get; set; }

        [JsonPropertyName("overlapDays")]
        [Required(ErrorMessage = "OverlapDays is required.")]
        [Range(1, 365, ErrorMessage = "OverlapDays must be between 1 and 365.")]
        public int OverlapDays { get; set; }

    }
}
