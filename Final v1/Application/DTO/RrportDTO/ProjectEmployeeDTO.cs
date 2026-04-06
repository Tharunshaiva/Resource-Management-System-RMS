using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.RrportDTO
{
    public class ProjectEmployeeDTO
    {
        //public int EmployeeID { get; set; }
        //public string FullName { get; set; }
        //public string Email { get; set; }
        //public string Designation { get; set; }

        //public string ExperienceInYears { get; set; }

        //public DateTime AllocationStartDate { get; set; }
        //public DateTime? AllocationEndDate { get; set; }
        //public int AllocationPercentage { get; set; }

        [JsonPropertyName("employeeID")]
        [Required(ErrorMessage = "EmployeeID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be greater than 0.")]
        public int EmployeeID { get; set; }

        [JsonPropertyName("fullName")]
        [Required(ErrorMessage = "FullName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; }

        [JsonPropertyName("designation")]
        [Required(ErrorMessage = "Designation is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters.")]
        public string Designation { get; set; }

        [JsonPropertyName("experienceInYears")]
        [Required(ErrorMessage = "ExperienceInYears is required.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Experience must be a valid number (e.g., 3 or 3.5).")]
        public string ExperienceInYears { get; set; }

        [JsonPropertyName("allocationStartDate")]
        [Required(ErrorMessage = "AllocationStartDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for AllocationStartDate.")]
        public DateTime AllocationStartDate { get; set; }

        [JsonPropertyName("allocationEndDate")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for AllocationEndDate.")]
        public DateTime? AllocationEndDate { get; set; }

        [JsonPropertyName("allocationPercentage")]
        [Required(ErrorMessage = "AllocationPercentage is required.")]
        [Range(1, 100, ErrorMessage = "AllocationPercentage must be between 1 and 100.")]
        public int AllocationPercentage { get; set; }
    }
}
