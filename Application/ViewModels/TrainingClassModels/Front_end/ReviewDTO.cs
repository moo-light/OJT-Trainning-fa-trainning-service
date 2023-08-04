using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class ReviewDTO
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; } = default!;
    }
}

