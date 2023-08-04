using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.QuizModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class SubmitQuizRepository : GenericRepository<SubmitQuiz>, ISubmitQuizRepository
    {
        private readonly AppDbContext _appDbContext;
        public SubmitQuizRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _appDbContext = context;
        }


        public List<ViewDetailResultDTO> GetViewDetail(Guid ViewID)
        {

            var result = from question in _appDbContext.Questions
                         join detailquiz in _appDbContext.DetailQuizQuestions on question.Id equals detailquiz.QuestionID
                         join quiz in _appDbContext.Quizzes on detailquiz.QuizID equals quiz.Id
                         join submitquiz in _appDbContext.SubmitQuiz on detailquiz.Id equals submitquiz.DetailQuizQuestionID
                         where quiz.Id == ViewID
                         select new ViewDetailResultDTO
                         {
                             DetailQuizQuestionID = detailquiz.Id,
                             Question = question.Content,
                             Answer1 = question.Answer1,
                             Answer2 = question.Answer2,
                             Answer3 = question.Answer3,
                             Answer4 = question.Answer4,
                             CorrectAnswer = question.CorrectAnswer
                         };

            return result.ToList();

        }

        public int CheckTrueAnswer(Guid userID, Guid QuizID)
        {

            var result = from quiz in _appDbContext.Quizzes
                         join detailquizquestion in _appDbContext.DetailQuizQuestions on quiz.Id equals detailquizquestion.QuizID
                         join submitquiz in _appDbContext.SubmitQuiz on detailquizquestion.Id equals submitquiz.DetailQuizQuestionID
                         where submitquiz.IsCorrect == true && quiz.Id == QuizID && submitquiz.UserID == userID
                         select submitquiz.UserID;

            return result.Count();
        }
    }
}
