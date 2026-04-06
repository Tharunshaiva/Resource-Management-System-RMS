using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Final_v1.Application.DTO.FlaskDTO
{
    public class BenchPredictionResponseDTO
    {
        //[JsonPropertyName("success")]
        //public bool Success { get; set; }

        //[JsonPropertyName("bench_probability")]
        //public string BenchProbability { get; set; }

        [JsonPropertyName("success")]
        [Required(ErrorMessage = "Success flag is required.")]
        public bool Success { get; set; }

        [JsonPropertyName("bench_probability")]
        [Required(ErrorMessage = "BenchProbability is required.")]
        [RegularExpression(@"^(Low|Medium|High)$", ErrorMessage = "BenchProbability must be Low, Medium, or High.")]
        public string BenchProbability { get; set; }
    }
}
