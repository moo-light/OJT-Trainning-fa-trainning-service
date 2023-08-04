using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Syllabus : BaseEntity
    {
        public string SyllabusName { get; set; }

        public string Code { get; set; }

        public string Status { get; set; }
        public string Level { get; set; }
        public string CourseObjective { get; set; }
        public string TechRequirements { get; set; }

        public double Duration { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public ICollection<Unit> Units { get; set; }

        public ICollection<DetailTrainingProgramSyllabus> DetailTrainingProgramSyllabus { get; set; }



    }
}
