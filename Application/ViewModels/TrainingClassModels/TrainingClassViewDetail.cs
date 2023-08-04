using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public class TrainingClassViewDetail
    {
        public Guid classId { get; set; }   
        public DateOrderForViewDetail dateOrder { get; set; }
        public string className { get; set; }
        public string classCode { get; set; }
        public string classStatus { get; set; }
        public DurationView classDuration { get; set; }
    }
}
