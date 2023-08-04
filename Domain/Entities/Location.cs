using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Location : BaseEntity
    {
        public string LocationName { get; set; } = default!;

        public ICollection<TrainingClass> TrainingClasses { get; set; }
        public ICollection<DetailTrainingClassParticipate> DetailTrainingClassesParticipate { get; set; }

    }
}
