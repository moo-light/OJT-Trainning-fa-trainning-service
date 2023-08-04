using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class TimeFrameDTO
    {
        [JsonPropertyName("start_date")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("end_date")]
        public DateTime EndDate { get; set; }
        [JsonPropertyName("highlighted_dates")]
        public ICollection<DateTime>? HighlightedDates { get; set; }
    }
}

