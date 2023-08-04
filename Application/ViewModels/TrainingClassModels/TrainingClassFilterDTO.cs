using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public partial class TrainingClassFilterDTO
    {
        public Guid ClassID { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreationDate { get; set; }
        public string LocationName { get; set; } = default!;
        public string Branch { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string Attendee { get; set; } = default!;
        public DurationView ClassDuration { get; set; } = default!;
        public LastEditDTO LastEditDTO { get; set; } = default!;
    }
    public partial class TrainingClassDTO : ExtendTrainingClassDTO
    {

    }
}
