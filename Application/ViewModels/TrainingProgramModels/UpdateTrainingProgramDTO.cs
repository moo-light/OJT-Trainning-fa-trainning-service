using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingProgramModels
{
    public class UpdateTrainingProgramDTO : CreateTrainingProgramDTO
    {
        public Guid? Id { get; set; }
    }
}
