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
    public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
    {
        private readonly AppDbContext _appDbContext;
        public QuizRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _appDbContext = context;
        }

        public async Task<List<DetailQuizQuestion>> GetAllQuestionByQuizTestId(Guid id)
        {
            var result = _appDbContext.DetailQuizQuestions.Where(x => x.QuizID == id).ToList();
            return result;
        }


        public List<ViewDoneQuizDTO> ViewAnswer(Guid QuizID, Guid UserID)
        {

            var result = from lecutre in _appDbContext.Lectures
                         join quiz in _appDbContext.Quizzes on lecutre.Id equals quiz.LectureID
                         join detailquiz in _appDbContext.DetailQuizQuestions on quiz.Id equals detailquiz.QuizID
                         join submitquiz in _appDbContext.SubmitQuiz on detailquiz.Id equals submitquiz.DetailQuizQuestionID
                         join question in _appDbContext.Questions on detailquiz.QuestionID equals question.Id
                         where quiz.Id == QuizID && submitquiz.UserID == UserID
                         select new ViewDoneQuizDTO
                         {
                             Question = question.Content,
                             Answer1 = question.Answer1,
                             Answer2 = question.Answer2,
                             Answer3 = question.Answer3,
                             Answer4 = question.Answer4,
                             CorrectAnswer = question.CorrectAnswer,
                             DetailQuizQuestionID = detailquiz.Id,
                             QuizID = quiz.Id,
                             UserAnswer = submitquiz.AnswerSubmit
                         };

            List<ViewDoneQuizDTO> listDto = new List<ViewDoneQuizDTO>();
            foreach (var dto in result)
            {
                listDto.Add(dto);
            }

            return listDto;
        }


    }
}
