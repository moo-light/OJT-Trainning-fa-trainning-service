using Application.ViewModels.TrainingClassModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingProgramModels.TrainingProgramView
{
    public class TrainingProgramViewForTrainingClassDetail
    {
        public Guid programId { get; set; }
        public string programName { get; set; }
        public DurationView programDuration { get; set; }
        public LastEditDTO lastEdit { get; set; }
    }
}
