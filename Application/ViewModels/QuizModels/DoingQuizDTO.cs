using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.QuizModels
{

    public class DoingQuizDTO
    {
        public int NumberOfQuestion { get; set; }

        public ICollection<ViewQuizForTrainer>? QuizQuestionDTOs { get; set; }

        public ICollection<AnswerQuizQuestionDTO>? AnswerQuizQuestionDTOs { get; set; }

    }
}
