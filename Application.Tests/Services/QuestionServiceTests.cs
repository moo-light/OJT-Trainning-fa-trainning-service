using Application.Interfaces;
using Application.Services;
using Application.ViewModels.QuizModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class QuestionServiceTests : SetupTest
    {
        private readonly IQuestionService _questionService;

        public QuestionServiceTests()
        {
            _questionService = new QuestionService(_claimsServiceMock.Object, _unitOfWorkMock.Object, _mapperConfig, _appConfigurationMock.Object);
        }

        [Fact]
        public async Task CreateEmptyQuiz_ShouldReturnTrue()
        {
            var emptyquiz = _fixture.Build<CreateEmptyQuizDTO>().With(x => x.LectureID).Create();
            var mock = _fixture.Build<Quiz>().Without(x => x.DetailQuizQuestion)
                                             .Without(x => x.Id)
                                             .Without(x => x.QuizName)
                                             .Without(x => x.CreatedBy).Without(x => x.ModificationBy)
                                             .Without(x => x.DeleteBy).Without(x => x.IsDeleted)
                                             .Without(x => x.DeletionDate)
                                             .Without(x => x.Lecture)
                                             .With(x => x.LectureID).With(x => x.NumberOfQuiz)
                                            .Create();
            _unitOfWorkMock.Setup(x => x.QuizRepository.AddAsync(It.IsAny<Quiz>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var result = await _questionService.CreateEmptyQuizTest(emptyquiz);
            _unitOfWorkMock.Verify(x => x.QuizRepository.AddAsync(It.IsAny<Quiz>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteQuizTest_ShouldReturnTrue()
        {
            var mock = _fixture.Build<Quiz>().Without(x => x.DetailQuizQuestion)
                                                  .With(x => x.Id)
                                              .Without(x => x.QuizName)
                                              .Without(x => x.CreatedBy).Without(x => x.ModificationBy)
                                              .Without(x => x.DeleteBy).Without(x => x.IsDeleted)
                                              .Without(x => x.DeletionDate)
                                              .Without(x => x.Lecture)
                                              .Without(x => x.LectureID).Without(x => x.NumberOfQuiz)
                                              .Without(x => x.CreationDate)
                                             .Without(x => x.ModificationDate)
                                              .Create();
            _unitOfWorkMock.Setup(x => x.QuizRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mock);

            var result = await _questionService.DeleteQuizTest(mock.Id);

            result.Should().BeTrue();
        }




    }
}
