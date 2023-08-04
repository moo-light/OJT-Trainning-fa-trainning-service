using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.FixViewSyllabus
{
    public class LessonDTO
    {
        public string Name { get; set; }

        public string OutputStandard { get; set; }

        public double Hours { get; set; }

        public string Status { get; set; }

        public string DeliveryType { get; set; }

    }
}
