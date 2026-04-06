using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class AssignEmployeeToProjectDto
    {
        //public string EmployeeName { get; set; }
        //public string ProjectName { get; set; }
        //public DateTime AllocationStartDate { get; set; }
        //public DateTime AllocationEndDate { get; set; }

        [JsonPropertyName("employeeName")]
        [Required(ErrorMessage = "EmployeeName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "EmployeeName cannot exceed 100 characters.")]
        public string EmployeeName { get; set; }

        [JsonPropertyName("projectName")]
        [Required(ErrorMessage = "ProjectName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "ProjectName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string ProjectName { get; set; }

        [JsonPropertyName("allocationStartDate")]
        [Required(ErrorMessage = "AllocationStartDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for AllocationStartDate.")]
        public DateTime AllocationStartDate { get; set; }

        [JsonPropertyName("allocationEndDate")]
        [Required(ErrorMessage = "AllocationEndDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for AllocationEndDate.")]
        public DateTime AllocationEndDate { get; set; }
    }
}
