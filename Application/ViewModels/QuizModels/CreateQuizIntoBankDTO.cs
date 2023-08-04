using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.QuizModels
{
    public class CreateQuizIntoBankDTO
    {
        //public Guid TopicID { get; set; }
        //public Topic Topic { get; set; }
        public Guid TopicID { get; set; }
        public int TypeID { get; set; }
        public string Content { get; set; }
        public string Answer1 { get; set; }
        public string Answer2 { get; set; }
        public string Answer3 { get; set; }
        public string Answer4 { get; set; }
        public string CorrectAnswer { get; set; }
        // public ICollection<QuizTest> QuizTest { get; set; }
    }
}
