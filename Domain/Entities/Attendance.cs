using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Attendance : BaseEntity
    {
        public DateTime Date { get; set; }

        public string Status { get; set; } = null!;

        public Guid? UserId { get; set; }

        public virtual User? User { get; set; }

        public Guid? TrainingClassId { get; set; }

        public virtual TrainingClass? TrainingClass { get; set; }

        public Guid? ApplicationId { get; set; }

        public virtual Applications? Application { get; set; }
    }
}
