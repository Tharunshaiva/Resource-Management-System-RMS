using System.ComponentModel.DataAnnotations;

namespace Final_v1.Application.DTO.SkillDTO
{
    public class CreateSkillDTO
    {
        //public int SkillID { get; set; }

        //public string SkillName { get; set; }
        //public string Description { get; set; }


        [Required(ErrorMessage = "SkillID is required")]
        public int SkillID { get; set; }

        [Required(ErrorMessage = "SkillName is required")]
        [StringLength(100, ErrorMessage = "SkillName cannot exceed 100 characters")]
        public string SkillName { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters")]
        public string Description { get; set; }

    }
}
