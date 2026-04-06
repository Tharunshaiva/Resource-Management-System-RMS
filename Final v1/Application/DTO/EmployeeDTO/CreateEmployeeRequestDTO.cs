using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class CreateEmployeeRequestDTO
    {
        //public string FullName { get; set; }
        //public string Email { get; set; }
        //public string Phone { get; set; }
        //public string Gender { get; set; }
        //public string Designation { get; set; }
        //public decimal ExperienceInYears { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Enetr valid FullName")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters")]
        public string Designation { get; set; }

        [Range(0, 50, ErrorMessage = "ExperienceInYears must be between 0 and 50")]
        public decimal ExperienceInYears { get; set; }
    }
}
