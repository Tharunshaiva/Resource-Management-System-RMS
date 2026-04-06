using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class ProjectSearchRequestDto
    {
        //public string? Status { get; set; } // 'Active'|'Inactive'|null
        //public string? NameContains { get; set; }
        //public DateTime? StartsOnOrAfter { get; set; }
        //public DateTime? EndsOnOrBefore { get; set; }
        //public int Page { get; set; } = 1;
        //public int PageSize { get; set; } = 20;

        [JsonPropertyName("status")]
        [RegularExpression("^(Active|Inactive)?$", ErrorMessage = "Status must be 'Active', 'Inactive', or null.")]
        public string? Status { get; set; } // 'Active'|'Inactive'|null => all

        [JsonPropertyName("nameContains")]
        [StringLength(100, ErrorMessage = "NameContains cannot exceed 100 characters.")]
        public string? NameContains { get; set; }

        [JsonPropertyName("startsOnOrAfter")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for StartsOnOrAfter.")]
        public DateTime? StartsOnOrAfter { get; set; }

        [JsonPropertyName("endsOnOrBefore")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format for EndsOnOrBefore.")]
        public DateTime? EndsOnOrBefore { get; set; }

        [JsonPropertyName("page")]
        [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
        public int Page { get; set; } = 1;

        [JsonPropertyName("pageSize")]
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 20;
    }
}
