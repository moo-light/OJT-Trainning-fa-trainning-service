using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.TrainingClassRelated
{
    public class TrainingClassAttendees
    {
        public Guid Id { get; set; }
        public Guid TrainingClassId { get; set; }
        public TrainingClass TrainingClass { get; set; }
        public int AttendeesPlannedNumber { get; set; }
        public int AttendeesAcceptedNumber { get; set; }
        public int AttendeesActualNumber { get; set; }
    }
}
