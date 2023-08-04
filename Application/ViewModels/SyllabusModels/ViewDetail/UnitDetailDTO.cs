using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.ViewDetail
{
    public class UnitDetailDTO
    {
        public string UnitName { get; set; }

        public float TotalTime { get; set; }

        public int Session { get; set; }

        //public Guid SyllabusID { get; set; }
        public List<LectureDTO> Lectures { get; set; }
    }
}
