using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.RrportDTO
{
    public class EmployeeAllocationHistoryDTO
    {
        //public int EmployeeId { get; set; }
        //public string ProjectName { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        //public int AllocationPercentage { get; set; }

        [JsonPropertyName("employeeId")]
        [Required(ErrorMessage = "EmployeeId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeId must be greater than 0.")]
        public int EmployeeId { get; set; }

        [JsonPropertyName("projectName")]
        [Required(ErrorMessage = "ProjectName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string ProjectName { get; set; }

        [JsonPropertyName("startDate")]
        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for StartDate.")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for EndDate.")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("allocationPercentage")]
        [Required(ErrorMessage = "AllocationPercentage is required.")]
        [Range(1, 100, ErrorMessage = "AllocationPercentage must be between 1 and 100.")]
        public int AllocationPercentage { get; set; }
    }
}
