using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingClassModels
{
    public  class TrainingClassViewAllDTO
    {
        public Guid? id { get; set; } = default!;
        public string? className { get; set; } = default!;
        public string? classCode { get; set; } = default!;
        public string? createdBy { get; set; } = default!;
        public DateTime? createdOn { get; set; }
        public DurationView? classDuration { get; set; } = default!;
        public string? location { get; set; } = default!;
        public string? fsu { get; set; } = default!;
        public string? attendee { get; set; } = default!;
       
    }
}
