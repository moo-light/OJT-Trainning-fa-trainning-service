using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface ISubmitQuizRepository : IGenericRepository<SubmitQuiz>
    {
        List<ViewDetailResultDTO> GetViewDetail(Guid ViewID);

        int CheckTrueAnswer(Guid userID, Guid QuizID);
    }
}
