using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Quiz : BaseEntity
    {

        //public string AnswerSubmit { get; set; }


        //public User user { get; set; }

        //public bool IsCorrect { get; set; }

        //Fk to another table
        public Lecture Lecture { get; set; }

        public Guid LectureID { get; set; }

        public int NumberOfQuiz { get; set; }

        public string? QuizName { get; set; }
        //public ICollection<CompareQuiz> CompareQuizzes { get; set; }
        //public ICollection<QuizBank> QuizBanks { get; set; }


        //public Guid DetailQuizQuestionId { get; set; }
        public ICollection<DetailQuizQuestion> DetailQuizQuestion { get; set; }

        //public ICollection<QuizTest> QuizTests { get; set; }


        //public ICollection<CompareQuiz> CompareQuizes { get; set; }
    }
}
