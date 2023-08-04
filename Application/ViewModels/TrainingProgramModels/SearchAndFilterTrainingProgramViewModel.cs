using Application.ViewModels.TrainingProgramModels.TrainingProgramView;

namespace Application.ViewModels.TrainingProgramModels
{
    public class SearchAndFilterTrainingProgramViewModel
    {
        public Guid? Id { get; set; }
        public string TrainingTitle { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DurationView Durations { get; set; } = default!;
        public string Status { get; set; } = default;
    }
}
