using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class TrainingProgramDTO
    {
        [JsonPropertyName("program_id")]
        public Guid TrainingProgramId { get; set; }
    }
}

