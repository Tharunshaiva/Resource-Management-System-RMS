using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class SkillAvailabilityDTO
    {
        //public int SkillID { get; set; }
        //public string SkillName { get; set; }
        //public int AvailableEmployees { get; set; }
        //public int AllocatedEmployees { get; set; }

        [Required(ErrorMessage = "SkillID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "SkillID must be greater than 0.")]
        public int SkillID { get; set; }

        [Required(ErrorMessage = "SkillName is required.")]
        [StringLength(100, ErrorMessage = "SkillName cannot exceed 100 characters.")]
        public string SkillName { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "AvailableEmployees cannot be negative.")]
        public int AvailableEmployees { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "AllocatedEmployees cannot be negative.")]
        public int AllocatedEmployees { get; set; }
    }
}
