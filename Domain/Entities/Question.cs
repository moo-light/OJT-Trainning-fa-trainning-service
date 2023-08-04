using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Question : BaseEntity
    {
        public string Content { get; set; }

        public string Answer1 { get; set; }

        public string Answer2 { get; set; }

        public string Answer3 { get; set; }

        public string Answer4 { get; set; }

        public string CorrectAnswer { get; set; }

        //public Guid QuizTestID { get; set; }
        //public ICollection<CompareQuiz> CompareQuizzes { get; set; }

        /// <summary>
        /// connect with topic ,1 topic have many quizbank , 1 quizbank belong 1 topc
        /// </summary>
        public Guid TopicID { get; set; }

        public Topic Topic { get; set; }


        //connect with quiz type 1 n
        public int QuizTypeID { get; set; }

        public QuizType QuizType { get; set; }

        public ICollection<DetailQuizQuestion> DetailQuizQuestion { get; set; }




    }
}
