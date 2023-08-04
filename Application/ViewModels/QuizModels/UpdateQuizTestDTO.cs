using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModels.QuizModels
{
    public class UpdateQuizTestDTO
    {
        public Guid IdQuestionWantToUpdate { get; set; }
        public Guid NewQuestionId { get; set; }
    }
}
