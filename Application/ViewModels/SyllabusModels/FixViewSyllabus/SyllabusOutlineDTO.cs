using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.FixViewSyllabus
{
    public class SyllabusOutlineDTO
    {
        public int Day { get; set; }

        public List<ContentSyllabusDTO> Content { get; set; }
    }
}
