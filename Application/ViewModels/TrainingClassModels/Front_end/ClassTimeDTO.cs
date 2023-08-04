using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class ClassTimeDTO
    {
        [JsonPropertyName("start_time")]
        public DateTime StartTime { get; set; }
        [JsonPropertyName("end_time")]
        public DateTime EndTime { get; set; }
    }
}

