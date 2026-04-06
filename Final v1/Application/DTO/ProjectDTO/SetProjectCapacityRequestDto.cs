using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class SetProjectCapacityRequestDto
    {
        //public int ProjectID { get; set; }
        //public int Capacity { get; set; }

        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0.")]
        public int ProjectID { get; set; }

        [JsonPropertyName("capacity")]
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")]
        public int Capacity { get; set; }

    }
}
