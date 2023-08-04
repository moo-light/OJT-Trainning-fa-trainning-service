using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Unit : BaseEntity
    {
        public string UnitName { get; set; }

        public double TotalTime { get; set; }

        public int Session { get; set; }

        public int UnitNum { get; set; }
        public ICollection<DetailUnitLecture> DetailUnitLectures { get; set; }

        public Guid SyllabusID { get; set; }

        public Syllabus Syllabus { get; set; }


    }
}
