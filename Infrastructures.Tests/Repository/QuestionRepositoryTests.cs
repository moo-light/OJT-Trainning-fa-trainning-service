using Application.Repositories;
using Application.ViewModels.QuizModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class QuestionRepositoryTests : SetupTest
    {
        private readonly QuestionRepository _questionRepository;
        private Quiz quiz_1;
        private Quiz quiz_2;
        private QuizType quizType;
        private Topic topic;
        private List<Question> questions;
        private List<DetailQuizQuestion> detailQuizQuestion;
        public QuestionRepositoryTests()
        {

            _questionRepository = new QuestionRepository(_dbContext, _currentTimeMock.Object, _claimsServiceMock.Object);
            #region Build Data
            quiz_1 = _fixture.Build<Quiz>().OmitAutoProperties()
               .With(x => x.Id, Guid.NewGuid())
               .With(x => x.QuizName)
               .Create();
            quiz_2 = _fixture.Build<Quiz>().OmitAutoProperties()
               .With(x => x.Id, Guid.NewGuid())
               .With(x => x.QuizName)
               .Create();
            quizType = _fixture.Build<QuizType>().OmitAutoProperties()
               .With(x => x.LevelType, 10)
               .With(x => x.NameType)
               .Create();
            topic = _fixture.Build<Topic>().OmitAutoProperties()
               .With(x => x.Id, Guid.NewGuid())
               .With(x => x.TopicName)
               .Create();

            questions = _fixture.Build<Question>().OmitAutoProperties()
               .With(x => x.Answer1)
               .With(x => x.Answer2)
               .With(x => x.Answer3)
               .With(x => x.Answer4)
               .With(x => x.Content)
               .With(x => x.TopicID, topic.Id)
               .With(x => x.QuizTypeID, quizType.LevelType)
               .CreateMany(10).ToList();
            questions.ForEach(x => x.Id = Guid.NewGuid());

            detailQuizQuestion = _fixture.Build<DetailQuizQuestion>().OmitAutoProperties()
               .With(x => x.QuizID, quiz_1.Id)
               .CreateMany(5).ToList();
            List<DetailQuizQuestion> detailQuizQuestion_2 = _fixture.Build<DetailQuizQuestion>().OmitAutoProperties()
                .With(x => x.QuizID, quiz_2.Id)
                .CreateMany(5).ToList();
            detailQuizQuestion.AddRange(detailQuizQuestion_2);
            for (int i = 0; i < detailQuizQuestion.Count; i++)
            {
                string correct = null;
                switch (i % 4 + 1)
                {
                    case 1: correct = questions[i].Answer1; break;
                    case 2: correct = questions[i].Answer2; break;
                    case 3: correct = questions[i].Answer3; break;
                    case 4: correct = questions[i].Answer4; break;
                }
                detailQuizQuestion[i].QuestionID = questions[i].Id;
                questions[i].CorrectAnswer = correct;
            }
            #endregion

            _dbContext.Add(quiz_1);
            _dbContext.Add(quiz_2);
            _dbContext.Add(quizType);
            _dbContext.Add(topic);
            _dbContext.AddRange(questions);
            _dbContext.AddRange(detailQuizQuestion);
            quiz_1.NumberOfQuiz = quiz_1.DetailQuizQuestion.Count;
            quiz_2.NumberOfQuiz = quiz_2.DetailQuizQuestion.Count;


            _dbContext.SaveChanges();
        }
        [Fact]
        public async Task GetQuizForTrainer_ShouldReturnCorrectValue()
        {
            // Setup


            // Act
            var result_1 = _questionRepository.GetQuizForTrainer(quiz_1.Id);
            var result_2 = _questionRepository.GetQuizForTrainer(quiz_2.Id);

            //Assert
            result_1.Count.Should().Be(5);
            result_1.ForEach(quiz => Assert.True(quiz.CorrectAnswer == quiz.Answer1
                                                 || quiz.CorrectAnswer == quiz.Answer2
                                                 || quiz.CorrectAnswer == quiz.Answer3
                                                 || quiz.CorrectAnswer == quiz.Answer4));
            result_1[0].QuizID.Should().Be(quiz_1.Id);
            result_1[0].DetailQuizQuestionID.Should().Be(detailQuizQuestion[0].Id);

            result_2.Count.Should().Be(5);
            result_2[0].QuizID.Should().Be(quiz_2.Id);
            result_2[0].DetailQuizQuestionID.Should().Be(detailQuizQuestion[5].Id);
        }
        [Fact]
        public async Task GetQuizForTrainee_ShouldReturnCorrectValue()
        {
            // Setup


            // Act
            var result_1 = _questionRepository.GetQuizForTrainee(quiz_1.Id);
            var result_2 = _questionRepository.GetQuizForTrainee(quiz_2.Id);

            //Assert
            result_1.Count.Should().Be(5);
            result_1.ForEach(quiz => quiz.UserAnswer.Should().Be(""));
            result_1[0].QuizID.Should().Be(quiz_1.Id);
            result_1[0].DetailQuizQuestionID.Should().Be(detailQuizQuestion[0].Id);

            result_2.Count.Should().Be(5);
            result_2[0].QuizID.Should().Be(quiz_2.Id);
            result_2[0].DetailQuizQuestionID.Should().Be(detailQuizQuestion[5].Id);
        }
    }
}
