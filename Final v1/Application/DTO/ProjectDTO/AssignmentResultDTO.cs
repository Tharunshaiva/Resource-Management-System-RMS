using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.ProjectDTO
{
    public class AssignmentResultDTO
    {
        //public int ResultCode { get; set; }
        //public string Message { get; set; } = string.Empty;

        [JsonPropertyName("resultCode")]
        [Required(ErrorMessage = "ResultCode is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "ResultCode must be a non-negative integer.")]
        public int ResultCode { get; set; }

        [JsonPropertyName("message")]
        [Required(ErrorMessage = "Message is required.")]
        [RegularExpression(@"\S+", ErrorMessage = "Message cannot be empty or whitespace.")]
        [StringLength(250, ErrorMessage = "Message cannot exceed 250 characters.")]
        public string Message { get; set; } = string.Empty;
    }
}
