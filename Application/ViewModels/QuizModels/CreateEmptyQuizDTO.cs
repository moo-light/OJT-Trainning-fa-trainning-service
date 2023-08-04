using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.ViewModels.QuizModels
{
    public class CreateEmptyQuizDTO
    {
        // public Guid UnitID { get; set; }
        // public Unit Unit { get; set; }
        // public int NumberOfQuiz { get; set; }
        public Guid LectureID { get; set; }
        // public ICollection<ListOfQuestionsDTO> Questions { get; set; }

        // public QuizBank QuizBank { get; set; }
        //public ICollection<SubmitQuiz> submitQuizzes { get; set; }
    }
}
