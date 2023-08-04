using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.TrainingClassRelated
{
    public class TrainingClassTimeFrame
    {
        public Guid Id { get; set; }
        public Guid TrainingClassId { get; set; }
        public TrainingClass TrainingClass { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<HighlightedDates>? HighlightedDates { get; set; }
    }
    public class HighlightedDates : BaseEntity
    {
        public Guid TrainingClassTimeFrameId { get; set; }
        public TrainingClassTimeFrame TrainingClassTimeFrame { get; set; }
        public DateTime HighlightedDate { get; set; }
    }
}
