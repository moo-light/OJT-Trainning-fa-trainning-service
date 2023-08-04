using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class AttendeesDTO
    {

        [JsonPropertyName("planned_number")]
        public int AttendeesPlannedNumber { get; set; }
        [JsonPropertyName("accepted_number")]
        public int AttendeesAcceptedNumber { get; set; }
        [JsonPropertyName("actual_number")]
        public int AttendeesActualNumber { get; set; }

    }
}

