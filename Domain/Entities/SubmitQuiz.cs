using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SubmitQuiz : BaseEntity
    {

        public string AnswerSubmit { get; set; }

        public bool IsCorrect { get; set; }

        //Fk to another table
        public Guid DetailQuizQuestionID { get; set; }

        public DetailQuizQuestion DetailQuizQuestion { get; set; }


        //public Guid GradingID { get; set; }

        //public Grading Grading { get; set;}

        public Guid UserID { get; set; }

        //public User User { get; set; }

        //public Guid UserID { get; set; }    

        //public User User { get; set; }


    }
}
