using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class EmployeeResponseDTO
    {
        //public int EmployeeID { get; set; }
        //public string FullName { get; set; }
        //public string Email { get; set; }
        //public string Phone { get; set; }
        //public string Gender { get; set; }
        //public DateTime DateOfJoining { get; set; }
        //public string Designation { get; set; }
        //public decimal ExperienceInYears { get; set; }
        //public bool IsActive { get; set; }

        //public List<string> Skills { get; set; } = new();

        //public List<int> SkillIDs { get; set; } = new();

        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be a positive number")]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "DateOfJoining is required")]
        public DateTime DateOfJoining { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters")]
        public string Designation { get; set; }

        [Range(0, 50, ErrorMessage = "ExperienceInYears must be between 0 and 50")]
        public decimal ExperienceInYears { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Skills list cannot be empty")]
        public List<string> Skills { get; set; } = new();

        [Required(ErrorMessage = "SkillIDs list cannot be empty")]
        public List<int> SkillIDs { get; set; } = new();

    }
}
