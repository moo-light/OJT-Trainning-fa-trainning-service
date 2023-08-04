using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.FixViewSyllabus
{
    public class ContentSyllabusDTO
    {
        public int UnitNum { get; set; }

        public string UnitName { get; set; }

        public double Hours { get; set; }

        public List<LessonDTO> Lessons { get; set; }
    }
}
