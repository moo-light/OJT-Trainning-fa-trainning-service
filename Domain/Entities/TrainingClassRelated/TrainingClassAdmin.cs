using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.TrainingClassRelated
{
    public class TrainingClassAdmin
    {
        public Guid Id { get; set; }
        public Guid AdminId { get; set; }
        public Guid TrainingClassId { get; set; }
        public TrainingClass TrainingClass { get; set; }
    }
}
