using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix
{
    public class UpdateSyllabusModel
    {
        public string SyllabusName { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Status { get; set; } = default!;

        public UpdateDuration Durations { get; set; } = default!;
        public UpdateGeneralModel General { get; set; } = default!;
        public List<UpdateOutlineModel> Outline { get; set; } = default!;
        public List<UpdateOtherModel> Others { get; set; } = default!;
    }
}
