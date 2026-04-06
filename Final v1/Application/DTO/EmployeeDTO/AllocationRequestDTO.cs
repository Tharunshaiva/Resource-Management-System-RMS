using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class AllocationRequestDTO
    {
        //public int EmployeeID { get; set; }
        //public int ProjectID { get; set; }
        //public DateTime AllocationStartDate { get; set; }
        //public DateTime AllocationEndDate { get; set; }
        //public int AllocationPercentage { get; set; } = 100;
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be a positive number")]
        public int EmployeeID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be a positive number")]
        public int ProjectID { get; set; }

        [Required(ErrorMessage = "AllocationStartDate is required")]
        public DateTime AllocationStartDate { get; set; }

        [Required(ErrorMessage = "AllocationEndDate is required")]
        public DateTime AllocationEndDate { get; set; }

        [Range(1, 100, ErrorMessage = "AllocationPercentage must be between 1 and 100")]
        public int AllocationPercentage { get; set; } = 100;
    }
}
