using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class SetProjectDatesRequestDto
    {
        //public int ProjectID { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }


        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0.")]
        public int ProjectID { get; set; }

        [JsonPropertyName("startDate")]
        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for StartDate.")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for EndDate.")]
        public DateTime EndDate { get; set; }

    }
}
