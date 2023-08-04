using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ClassSchedule : BaseEntity
    {
        public Guid? TrainingClassId { get; set; }

        public string StatusClassSchedule { get; set; } = default!;

        public DateTime ClassStartTime { get; set; }

        public DateTime ClassEndTime { get; set; }

        //public string OnlineClassLink { get; set; }
        public string RoomName { get; set; }

        public virtual TrainingClass TrainingClasses { get; set; }
    }
}
