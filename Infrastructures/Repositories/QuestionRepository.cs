using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.QuizModels;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private readonly AppDbContext _appDbContext;
        public QuestionRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _appDbContext = context;
        }

        // using for trainer to view quiz
        public List<ViewQuizForTrainer> GetQuizForTrainer(Guid QuizID)
        {

            var result = from question in _appDbContext.Questions
                         join detailquiz in _appDbContext.DetailQuizQuestions on question.Id equals detailquiz.QuestionID
                         join quiz in _appDbContext.Quizzes on detailquiz.QuizID equals quiz.Id
                         where quiz.Id == QuizID
                         select new ViewQuizForTrainer
                         {
                             QuizID = quiz.Id,
                             DetailQuizQuestionID = detailquiz.Id,
                             Question = question.Content,
                             Answer1 = question.Answer1,
                             Answer2 = question.Answer2,
                             Answer3 = question.Answer3,
                             Answer4 = question.Answer4,
                             CorrectAnswer = question.CorrectAnswer,
                         };

            return result.ToList();

        }

        //Add for trainnee to make quix
        public List<AnswerQuizQuestionDTO> GetQuizForTrainee(Guid QuizID)
        {

            var result = from question in _appDbContext.Questions
                         join detailquiz in _appDbContext.DetailQuizQuestions on question.Id equals detailquiz.QuestionID
                         join quiz in _appDbContext.Quizzes on detailquiz.QuizID equals quiz.Id
                         where quiz.Id == QuizID
                         select new AnswerQuizQuestionDTO
                         {
                             QuizID = quiz.Id,
                             DetailQuizQuestionID = detailquiz.Id,
                             Question = question.Content,
                             Answer1 = question.Answer1,
                             Answer2 = question.Answer2,
                             Answer3 = question.Answer3,
                             Answer4 = question.Answer4,
                             UserAnswer = "",

                         };

            return result.ToList();

        }

    }

}
