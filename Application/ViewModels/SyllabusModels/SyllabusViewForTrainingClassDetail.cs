using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels
{
    public class SyllabusViewForTrainingClassDetail
    {
        public Guid syllabus_id { get; set; }
        public ICollection<string> trainerAvatarsUrl { get; set; }
        public string syllabus_name { get; set; }
        public string syllabus_status { get; set; }
        public DurationView syllabus_duration { get; set; } 
        public LastEditDTO lastEdit { get; set; }   
    }
}
