using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.RrportDTO
{
    public class ProjectUtilizationDTO
    {
        //public int ProjectId { get; set; }
        //public string ProjectName { get; set; }
        //public decimal Utilization { get; set; }

        [JsonPropertyName("projectId")]
        [Required(ErrorMessage = "ProjectId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectId must be greater than 0.")]
        public int ProjectId { get; set; }

        [JsonPropertyName("projectName")]
        [Required(ErrorMessage = "ProjectName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string ProjectName { get; set; }

        [JsonPropertyName("utilization")]
        [Required(ErrorMessage = "Utilization is required.")]
        [Range(0, 100, ErrorMessage = "Utilization must be between 0 and 100.")]
        public decimal Utilization { get; set; }


    }
}
