using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.SkillDTO
{
    public class EmployeeSkillMappingDTO
    {
        //public string EmployeeName { get; set; }
        //public string SkillName { get; set; }


        [Required(ErrorMessage = "EmployeeName is required")]
        [StringLength(100, ErrorMessage = "EmployeeName cannot exceed 100 characters")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "SkillName is required")]
        [StringLength(100, ErrorMessage = "SkillName cannot exceed 100 characters")]
        public string SkillName { get; set; }

    }
}
