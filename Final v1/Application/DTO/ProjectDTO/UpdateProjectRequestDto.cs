using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class UpdateProjectRequestDto
    {
        //public int ProjectID { get; set; }
        //public string? ProjectName { get; set; }
        //public string? ClientName { get; set; }
        //public int? Capacity { get; set; }
        //public DateTime? StartDate { get; set; }
        //public DateTime? EndDate { get; set; }
        //public string? ProjectStatus { get; set; } // optional

        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0.")]
        public int ProjectID { get; set; }

        [JsonPropertyName("projectName")]
        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string? ProjectName { get; set; }

        [JsonPropertyName("clientName")]
        [StringLength(100, ErrorMessage = "ClientName cannot exceed 100 characters.")]
        public string? ClientName { get; set; }

        [JsonPropertyName("capacity")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")]
        public int? Capacity { get; set; }

        [JsonPropertyName("startDate")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for StartDate.")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for EndDate.")]
        public DateTime? EndDate { get; set; }

        [JsonPropertyName("projectStatus")]
        [RegularExpression("^(Active|Inactive)?$", ErrorMessage = "ProjectStatus must be 'Active', 'Inactive', or null.")]
        public string? ProjectStatus { get; set; } // optional
    }
}
