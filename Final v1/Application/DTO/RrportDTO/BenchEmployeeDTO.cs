using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.RrportDTO
{
    public class BenchEmployeeDTO
    {
        //public int EmployeeID { get; set; }
        //public string FullName { get; set; }
        //public string Designation { get; set; }
        //public decimal ExperienceInYears { get; set; }
        //public string Skills { get; set; }

        [JsonPropertyName("employeeID")]
        [Required(ErrorMessage = "EmployeeID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be greater than 0.")]
        public int EmployeeID { get; set; }

        [JsonPropertyName("fullName")]
        [Required(ErrorMessage = "FullName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [JsonPropertyName("designation")]
        [Required(ErrorMessage = "Designation is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "EmployeeName cannot be empty or whitespace.")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters.")]
        public string Designation { get; set; }

        [JsonPropertyName("experienceInYears")]
        [Range(0, 50, ErrorMessage = "ExperienceInYears must be between 0 and 50.")]
        public decimal ExperienceInYears { get; set; }

        [JsonPropertyName("skills")]
        [Required(ErrorMessage = "Skills are required.")]
        [StringLength(250, ErrorMessage = "Skills cannot exceed 250 characters.")]
        public string Skills { get; set; }
    }
}
