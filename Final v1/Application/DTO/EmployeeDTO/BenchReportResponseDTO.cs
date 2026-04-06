using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class BenchReportResponseDTO
    {
        //public int EmployeeID { get; set; }
        //public string FullName { get; set; }
        //public string Designation { get; set; }
        //public decimal ExperienceInYears { get; set; }
        //public string Skills { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be a positive number")]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters")]
        public string Designation { get; set; }

        [Range(0, 50, ErrorMessage = "ExperienceInYears must be between 0 and 50")]
        public decimal ExperienceInYears { get; set; }

        [Required(ErrorMessage = "Skills are required")]
        [StringLength(500, ErrorMessage = "Skills cannot exceed 500 characters")]
        public string Skills { get; set; }
    }
}
