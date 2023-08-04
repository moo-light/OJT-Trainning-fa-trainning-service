using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class ApproveDTO
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
        [JsonPropertyName("author")]
        public string Author { get; set; } = default!;
    }
}

