using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingProgramModels
{
    public class TrainingProgramViewModel
    {
        public Guid? TrainingProgramId { get; set; }
        public string TrainingTitle { get; set; } = default!;
        public string TrainingStatus { get; set; } = default;
        public DurationView TrainingDuration { get; set; } = default!;
        public ModifiedView Modified { get; set; } = default!;
        public string CreateByUserName { get; set; } = default!;
        public Guid CreatedBy { get; set; }
        public ICollection<SyllabusTrainingProgramViewModel>? Contents { get; set; }
    }
}
