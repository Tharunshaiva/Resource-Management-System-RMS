//namespace Final_v1.Application.DTO.ProjectDTO
//{
//    public class ProjectResponseDto
//    {
//        public int ProjectID { get; set; }
//        public string ProjectName { get; set; } = string.Empty;
//        public string ClientName { get; set; } = string.Empty;
//        public int Capacity { get; set; }
//        public DateTime StartDate { get; set; }
//        public DateTime EndDate { get; set; }
//        public string ProjectStatus { get; set; } = "Active";

//        public static ProjectResponseDto FromEntity(Final_v1.Domain.Entites.Project p)
//            => new ProjectResponseDto
//            {
//                ProjectID = p.ProjectID,
//                ProjectName = p.ProjectName,
//                ClientName = p.ClientName,
//                Capacity = p.Capacity,
//                StartDate = p.StartDate,
//                EndDate = p.EndDate,
//                ProjectStatus = p.ProjectStatus
//            };
//    }
//}


using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class ProjectResponseDto
    {
        [JsonPropertyName("projectID")]
        [Required(ErrorMessage = "ProjectID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0.")]
        public int ProjectID { get; set; }

        [JsonPropertyName("projectName")]
        [Required(ErrorMessage = "ProjectName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "ProjectName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "ProjectName cannot exceed 100 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        [JsonPropertyName("clientName")]
        [Required(ErrorMessage = "ClientName is required.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "ClientName cannot be empty or whitespace.")]
        [StringLength(100, ErrorMessage = "ClientName cannot exceed 100 characters.")]
        public string ClientName { get; set; } = string.Empty;

        [JsonPropertyName("capacity")]
        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")]
        public int Capacity { get; set; }

        [JsonPropertyName("startDate")]
        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for StartDate.")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for EndDate.")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("projectStatus")]
        [Required(ErrorMessage = "ProjectStatus is required.")]
        [RegularExpression("^(Active|Inactive)$", ErrorMessage = "ProjectStatus must be 'Active' or 'Inactive'.")]
        public string ProjectStatus { get; set; } = "Active";

        public int Assigned { get; set; }

        public static ProjectResponseDto FromEntity(Final_v1.Domain.Entites.Project p)
    => new ProjectResponseDto
    {
        ProjectID = p.ProjectID,
        ProjectName = p.ProjectName,
        ClientName = p.ClientName,
        Capacity = p.Capacity,
        StartDate = p.StartDate,
        EndDate = p.EndDate,
        ProjectStatus = p.ProjectStatus,
        Assigned = p.Assigned
    };
    }
}
