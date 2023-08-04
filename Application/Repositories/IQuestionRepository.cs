using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {

        public List<ViewQuizForTrainer> GetQuizForTrainer(Guid QuizID);

        public List<AnswerQuizQuestionDTO> GetQuizForTrainee(Guid QuizID);



    }
}
