using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.QuizModels
{
    public class FilterQuizModel
    {
        public ICollection<int> QuizType { get; set; }

        public ICollection<Guid> QuizTopic { get; set; }
    }
}
