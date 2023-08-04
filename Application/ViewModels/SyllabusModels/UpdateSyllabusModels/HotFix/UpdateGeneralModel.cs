using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix
{
    public class UpdateGeneralModel
    {
        public string Level { get; set; } = default!;
        public double AttendeeNumberPercent { get; set; }
        public string OutputStandard { get; set; } = default!;
        public string TechnicalRequirements { get; set; } = default!;
        public string CourseObjective { get; set; } = default!;

    }
}
