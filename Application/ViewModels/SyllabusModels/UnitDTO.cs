using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels
{
    public class UnitDTO
    {
        public string UnitName { get; set; }

        public float TotalTime { get; set; }

        public int Session { get; set; }

        //public Guid SyllabusID { get; set; }
        public ICollection<LectureDTO> Lectures { get; set; }


    }
}
