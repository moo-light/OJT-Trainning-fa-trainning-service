using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix
{
    public class UpdateLessonModel
    {
        public string Name { get; set; } = default!;
        public string OutputStandards { get; set; } = default!;
        public double Hours { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string DeliveryType { get; set; } = default!;

    }
}
