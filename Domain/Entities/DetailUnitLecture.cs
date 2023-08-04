using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DetailUnitLecture : BaseEntity
    {
        public Guid UnitId { get; set; }

        public Unit Unit { get; set; }

        public Guid LectureID { get; set; }

        public Lecture Lecture { get; set; }
    }
}
