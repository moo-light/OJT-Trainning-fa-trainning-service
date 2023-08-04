using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.TrainingProgramModels.TrainingProgramView
{
    public class DurationView
    {
        public double TotalDate { get { return TotalHours / 8; } }
        public double TotalHours { get; set; } = 10;

    }
}
