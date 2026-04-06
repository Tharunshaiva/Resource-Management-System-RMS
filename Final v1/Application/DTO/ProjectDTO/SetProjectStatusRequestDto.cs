using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class SetProjectStatusRequestDto
    {
        //public int ProjectID { get; set; }
        //public string ProjectStatus { get; set; } = "Inactive";


        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0.")]
        public int ProjectID { get; set; }

        [JsonPropertyName("projectStatus")]
        [Required(ErrorMessage = "ProjectStatus is required.")]
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "ProjectStatus must be 'Active' or 'Inactive'.")]
        public string ProjectStatus { get; set; } = "Inactive";

    }
}
