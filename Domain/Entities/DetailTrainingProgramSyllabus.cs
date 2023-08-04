using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DetailTrainingProgramSyllabus : BaseEntity
    {
        public Guid SyllabusId { get; set; }

        public Syllabus Syllabus { get; set; }

        public Guid TrainingProgramId { get; set; }

        public TrainingProgram TrainingProgram { get; set; }

        public string Status { get; set; }
    }
}
