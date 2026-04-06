using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.AllocationDTO
{
    public class AllocationHistoryDto
    {
        //public int AllocationID { get; set; }
        //public string ProjectName { get; set; }
        //public DateTime AllocationStartDate { get; set; }
        //public DateTime? AllocationEndDate { get; set; }
        //public int AllocationPercentage { get; set; }

        [Required(ErrorMessage = "AllocationID is required")]
        public int AllocationID { get; set; }

        [Required(ErrorMessage = "ProjectName is required")]
        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters")]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "AllocationStartDate is required")]
        public DateTime AllocationStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AllocationEndDate { get; set; }

        [Range(1, 100, ErrorMessage = "AllocationPercentage must be between 1 and 100")]
        public int AllocationPercentage { get; set; }
    }

    }
