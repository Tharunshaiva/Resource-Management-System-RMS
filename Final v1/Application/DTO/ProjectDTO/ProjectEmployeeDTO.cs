using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class ProjectEmployeeDTO
    {
        //public int EmployeeID { get; set; }
        //public string FullName { get; set; } = string.Empty;
        //public string Email { get; set; } = string.Empty;
        //public string Designation { get; set; } = string.Empty;
        //public float ExperienceInYears { get; set; }

        [JsonPropertyName("employeeID")]
        [Required(ErrorMessage = "EmployeeID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be greater than 0.")]
        public int EmployeeID { get; set; }

        [JsonPropertyName("fullName")]
        [Required(ErrorMessage = "FullName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "ProjectName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("designation")]
        [Required(ErrorMessage = "Designation is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Designation cannot be empty or whitespace.")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters.")]
        public string Designation { get; set; } = string.Empty;

        [JsonPropertyName("experienceInYears")]
        [Range(0, 50, ErrorMessage = "ExperienceInYears must be between 0 and 50.")]
        public float ExperienceInYears { get; set; }
    }
}
