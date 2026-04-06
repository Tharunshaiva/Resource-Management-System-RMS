using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class SoftDeleteEmployeeRequestDTO
    {

        [Required(ErrorMessage = "IsActive status is required.")]
        public bool IsActive { get; set; }  // false = deactivate
    }
}
