using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingProgramModels
{
    public class CreateTrainingProgramDTO
    {
        public string ProgramName { get; set; } = default!;
        public ICollection<Guid>? SyllabusesId { get; set; }
    }
}
