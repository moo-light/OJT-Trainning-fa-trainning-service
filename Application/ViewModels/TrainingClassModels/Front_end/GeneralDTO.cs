using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class GeneralDTO
    {
        [JsonPropertyName("class_time")]

        public ClassTimeDTO ClassTime { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("admins")]
        public ICollection<AdminsDTO> Admins { get; set; } = default!;
        [JsonPropertyName("trainer")]
        public ICollection<TrainerDTO> Trainers { get; set; } = default!;
        [JsonPropertyName("fsu")]
        public string Fsu { get; set; }

        //ReviewDTO
        [JsonPropertyName("review")]
        public ReviewDTO? Review { get; set; }
        //ApproveDTO
        [JsonPropertyName("approve")]
        public ApproveDTO? Approve { get; set; }
    }
}

