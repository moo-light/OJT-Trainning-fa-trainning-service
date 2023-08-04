using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class TrainingProgram : BaseEntity
    {
        public string ProgramName { get; set; } = default!;

        public string Status { get; set; } = default!;

        public double Duration { get; set; }
        public ICollection<DetailTrainingProgramSyllabus> DetailTrainingProgramSyllabus { get; set; } = default!;

        public ICollection<TrainingClass> TrainingClasses { get; set; } = default!;
    }
}
