using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingProgramModels.TrainingProgramView
{
    public class SyllabusTrainingProgramViewModel
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = default!;
        public string SyllabusName { get; set; } = default!;
        public DurationView SyllabusDuration { get; set; } = default!;
        public ModifiedView Modified { get; set; } = default!;


    }
}
