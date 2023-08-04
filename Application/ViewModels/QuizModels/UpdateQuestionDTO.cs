using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.QuizModels
{
    public class UpdateQuestionDTO : CreateQuizIntoBankDTO
    {
        public Guid QuestionId { get; set; }
    }
}
