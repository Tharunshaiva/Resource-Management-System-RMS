using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class ProjectUtilizationResponseDTO
    {
        //public int ProjectID { get; set; }
        //public string ProjectName { get; set; }
        //public int Capacity { get; set; }
        //public int AssignedCount { get; set; }
        //public int AvailableSlots { get; set; }

        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0.")]
        public int ProjectID { get; set; }

        [JsonPropertyName("projectName")]
        [Required(ErrorMessage = "ProjectName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "ProjectName cannot be empty or whitespace.")]

        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string ProjectName { get; set; }

        [JsonPropertyName("capacity")]
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")]
        public int Capacity { get; set; }

        [JsonPropertyName("assignedCount")]
        [Required(ErrorMessage = "AssignedCount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "AssignedCount cannot be negative.")]
        public int AssignedCount { get; set; }

        [JsonPropertyName("availableSlots")]
        [Required(ErrorMessage = "AvailableSlots is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "AvailableSlots cannot be negative.")]
        public int AvailableSlots { get; set; }
    }
}
