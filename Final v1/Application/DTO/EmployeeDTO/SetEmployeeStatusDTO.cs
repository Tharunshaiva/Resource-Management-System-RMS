using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class SetEmployeeStatusDTO
    {
        //public int EmployeeID { get; set; }
        //public bool IsActive { get; set; }

        [Required(ErrorMessage = "EmployeeID is required.")]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "IsActive status is required.")]
        public bool IsActive { get; set; }


    }
}
