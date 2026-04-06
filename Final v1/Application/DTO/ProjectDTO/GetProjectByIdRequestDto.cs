using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class GetProjectByIdRequestDto
    {
        //public int ProjectID { get; set; }
        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0.")]
        public int ProjectID { get; set; }

    }
}
