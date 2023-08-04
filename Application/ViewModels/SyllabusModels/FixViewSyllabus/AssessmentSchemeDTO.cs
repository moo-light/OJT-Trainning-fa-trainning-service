using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.SyllabusModels.FixViewSyllabus
{
    public class AssessmentSchemeDTO
    {
        public int QuizPercent { get; set; }

        public int AssignmentPercent { get; set; }

        public int FinalPercent { get; set; }


        public int FinalTheoryPercent { get; set; }

        public int FinalPreactisePercent { get; set; }

        public int GPAPercent { get; set; }
    }
}
