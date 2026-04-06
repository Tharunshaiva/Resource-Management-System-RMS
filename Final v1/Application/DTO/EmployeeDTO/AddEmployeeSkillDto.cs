using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.EmployeeDTO
{
    public class AddEmployeeSkillDto
    {
        //public int EmployeeID { get; set; }
        //public int SkillID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be a positive number")]
        public int EmployeeID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SkillID must be a positive number")]
        public int SkillID { get; set; }

    }
}
