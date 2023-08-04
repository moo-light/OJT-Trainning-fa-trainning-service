using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.QuizModels
{
    public class AddQuestionToQuizTestDTO
    {
        public Guid QuizId { get; set; }
        public Guid QuestionId { get; set; }
        //public Guid SubmitQuiz { get; set; }
    }
}
