using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix
{
    public class UpdateContentModel
    {
        public int Unit { get; set; }
        public string UnitName { get; set; } = default!;
        public double Hours { get; set; }
        public List<UpdateLessonModel> Lessons { get; set; } = default!;

    }
}
