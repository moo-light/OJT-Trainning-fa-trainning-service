using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix
{
    public class UpdateAssesmentScheme
    {
        public double QuizPercent { get; set; } = 15;
        public double AssignmentPercent { get; set; } = 15;
        public double FinalPercent { get; set; } = 70;
        public double FinalTheoryPercent { get; set; } = 40;
        public double FinalPracticePercent { get; set; } = 60;
        public double GPAPercent { get; set; } = 70;
    }
}
