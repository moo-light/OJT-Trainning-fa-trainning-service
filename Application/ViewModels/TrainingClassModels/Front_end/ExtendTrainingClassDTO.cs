using System.Text.Json.Serialization;

namespace Application.ViewModels.TrainingClassModels
{
    public class ExtendTrainingClassDTO2
    {
        [JsonPropertyName("general")]
        public GeneralDTO General { get; set; }
        //Time frame
        [JsonPropertyName("time_frame")]
        public TimeFrameDTO TimeFrame { get; set; } = default!;
        //AttendeesDTO
        [JsonPropertyName("attendees")]
        public AttendeesDTO Attendees { get; set; } = default!;
        [JsonPropertyName("training_programs")]
        public TrainingProgramDTO TrainingPrograms { get; set; } = default!;
    }
    public class ExtendTrainingClassDTO
    {
        public ICollection<AdminsDTO> Admins { get; set; } = default!;
        public ICollection<TrainerDTO> Trainers { get; set; } = default!;
        public string Fsu { get; set; }
        //Time frame
        public TimeFrameDTO TimeFrame { get; set; } = default!;
        //AttendeesDTO
        public AttendeesDTO Attendees { get; set; } = default!;
        public DateTime ReviewDate { get; set; }
        public string ReviewAuthor { get; set; }
        public DateTime ApproveDate { get; set; }
        public string ApproveAuthor { get; set; }
    }
}

