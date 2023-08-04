using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DetailQuizQuestion : BaseEntity
    {


        public Quiz Quiz { get; set; }

        public Guid QuizID { get; set; }

        public Guid QuestionID { get; set; }

        public Question Question { get; set; }

        //public SubmitQuiz SubmitQuiz { get; set; }

        //public Guid SubmitQuizID { get; set; }


        public ICollection<SubmitQuiz> submitQuiz { get; set; }

        //public Guid SubmitQuizID { get; set; }

    }
}
