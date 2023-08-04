using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.QuizModels
{
    public class AnswerQuizDetailTraineeDTO
    {
        public string QuizName { get; set; }

        public int NumberOfQuiz { get; set; }

        public ICollection<ViewDoneQuizDTO>? DoneQuiz { get; set; }

    }
}
