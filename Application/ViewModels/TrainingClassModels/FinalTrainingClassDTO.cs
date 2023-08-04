using Application.ViewModels.SyllabusModels;
using Application.ViewModels.TrainingProgramModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public class FinalTrainingClassDTO
    {
        public Guid classId { get; set; }
        public DateOrderForViewDetail dateOrder { get; set; }
        public string className { get; set; }
        public string classCode { get; set; }
        public string classStatus { get; set; }
        public DurationView classDuration { get; set; }
        public GeneralTrainingClassDTO general { get; set; }
        public string location { get; set; }
        public ICollection<ClassTrainerDTO> trainer { get; set; }
        public ICollection<ClassAdminDTO> admin { get; set; }
        public string fsu { get; set; }
        public CreatedByDTO created{ get; set; }
        public TrainingClassReview review { get; set; }
        public AttendeeDTO attendeesDetail { get; set; }
        public TrainingProgramViewForTrainingClassDetail trainingPrograms { get; set; }
        public LastEditDTO lastEdit { get; set; }
        public ICollection<SyllabusViewForTrainingClassDetail> syllabuses  { get; set; }
    }
}
