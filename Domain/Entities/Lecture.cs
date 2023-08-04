using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Lecture : BaseEntity
    {
        public string LectureName { get; set; }

        public string OutputStandards { get; set; }

        public double Duration { get; set; }

        public string DeliveryType { get; set; }

        public string Status { get; set; }

        public ICollection<DetailUnitLecture> DetailUnitLectures { get; set; }

        public ICollection<TrainingMaterial> TrainingMaterials { get; set; }

        public ICollection<Grading> Gradings { get; set; }

        /// <summary>
        /// One lecture may have one assignmnet
        /// </summary>
        public ICollection<Assignment> Assignments { get; set; }
        public ICollection<AuditPlan> AuditPlans { get; set; } = default!;

        public Quiz? Quiz { get; set; }

        public Guid? QuizID { get; set; }

    }
}
