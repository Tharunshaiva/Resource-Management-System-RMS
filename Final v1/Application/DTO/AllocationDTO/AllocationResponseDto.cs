using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.AllocationDTO
{
    public class AllocationResponseDto
    {
        //public int AllocationID { get; set; }
        //public string EmployeeName { get; set; }
        //public string SkillName { get; set; }
        //public int EmployeeID { get; set; }
        //public int ProjectID { get; set; }
        //public DateTime AllocationStartDate { get; set; }
        //public DateTime? AllocationEndDate { get; set; }

        [Required(ErrorMessage = "AllocationID is required")]
        public int AllocationID { get; set; }

        [Required(ErrorMessage = "EmployeeName is required")]
        [StringLength(100, ErrorMessage = "EmployeeName cannot exceed 100 characters")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "SkillName is required")]
        [StringLength(100, ErrorMessage = "SkillName cannot exceed 100 characters")]
        public string SkillName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be a positive number")]
        public int EmployeeID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be a positive number")]
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "AllocationStartDate is required")]
        public DateTime AllocationStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AllocationEndDate { get; set; }
    }
}
