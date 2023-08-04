using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IQuizRepository : IGenericRepository<Quiz>
    {
        public Task<List<DetailQuizQuestion>> GetAllQuestionByQuizTestId(Guid id);

        public List<ViewDoneQuizDTO> ViewAnswer(Guid QuizID, Guid UserID);
    }
}
