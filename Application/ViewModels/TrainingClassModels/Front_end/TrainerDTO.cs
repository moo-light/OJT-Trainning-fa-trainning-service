using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class TrainerDTO
    {

        [JsonPropertyName("trainer_id")]
        public Guid TrainerId { get; set; }
    }
}

