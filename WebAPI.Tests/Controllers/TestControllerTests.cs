using Application.ViewModels.QuizModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class TestControllerTests : SetupTest
    {
        public readonly TestController _testController;
        public TestControllerTests()
        {
            _testController = new TestController(_quizServiceMock.Object);
        }
        [Fact]
        public async Task AddQuestionIntoBank_ShouldReturnCorrectValue()
        {
            // Setup
            _quizServiceMock.Setup(x => x.AddQuestionToBank(It.IsNotNull<CreateQuizIntoBankDTO>())).ReturnsAsync(true);
            _quizServiceMock.Setup(x => x.AddQuestionToBank(It.Is<CreateQuizIntoBankDTO>(x => x == null))).ReturnsAsync(false);
            // Act
            var result = await _testController.AddQuestionIntoBank(new CreateQuizIntoBankDTO());
            var result_badRequest = await _testController.AddQuestionIntoBank(null);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task CreateEmptyQuizTest_ShouldReturnCorrectValue()
        {
            // Setup
            _quizServiceMock.Setup(x => x.CreateEmptyQuizTest(It.IsNotNull<CreateEmptyQuizDTO>())).ReturnsAsync(true);
            _quizServiceMock.Setup(x => x.CreateEmptyQuizTest(It.Is<CreateEmptyQuizDTO>(x => x == null))).ReturnsAsync(false);
            // Act
            var result = await _testController.CreateEmptyQuizTest(new CreateEmptyQuizDTO());
            var result_badRequest = await _testController.CreateEmptyQuizTest(null);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task AddQuestionToQuizTest_ShouldReturnCorrectValue()
        {
            // Setup
            _quizServiceMock.Setup(x => x.AddQuestionToQuizTest(It.IsNotNull<AddQuestionToQuizTestDTO>())).ReturnsAsync(true);
            _quizServiceMock.Setup(x => x.AddQuestionToQuizTest(It.Is<AddQuestionToQuizTestDTO>(x => x == null))).ReturnsAsync(false);
            // Act
            var result = await _testController.AddQuestionToQuizTest(new AddQuestionToQuizTestDTO());
            var result_badRequest = await _testController.AddQuestionToQuizTest(null);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task UpdateQuizTest_ShouldReturnCorrectValue()
        {
            // Setup
            _quizServiceMock.Setup(x => x.UpdateQuizTest(It.IsAny<Guid>(), It.IsNotNull<UpdateQuizTestDTO>())).ReturnsAsync(true);
            _quizServiceMock.Setup(x => x.UpdateQuizTest(It.IsAny<Guid>(), It.Is<UpdateQuizTestDTO>(x => x == null))).ReturnsAsync(false);
            // Act
            var result = await _testController.UpdateQuizTest(Guid.NewGuid(), new UpdateQuizTestDTO());
            var result_badRequest = await _testController.UpdateQuizTest(Guid.NewGuid(), null);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DeleteQuizTest_ShouldReturnCorrectValue()
        {
            // Setup
            _quizServiceMock.Setup(x => x.DeleteQuizTest(It.IsAny<Guid>())).ReturnsAsync(true);
            _quizServiceMock.Setup(x => x.DeleteQuizTest(It.Is<Guid>(x => x == Guid.Empty))).ReturnsAsync(false);
            // Act
            var result = await _testController.DeleteQuizTest(Guid.NewGuid());
            var result_badRequest = await _testController.DeleteQuizTest(Guid.Empty);
            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task SearchByName_ShouldReturnCorrectValue()
        {
            // Setup
            var listName = new List<CreateQuizIntoBankDTO>();
            _quizServiceMock.Setup(x => x.Search(It.IsAny<string>())).ReturnsAsync(null as List<CreateQuizIntoBankDTO>);
            _quizServiceMock.Setup(x => x.Search(It.Is<string>(x => x == ""))).ReturnsAsync(listName);

            // Act
            var result = await _testController.SearchByName("");
            var result_badRequest = await _testController.SearchByName("comsuon");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(listName);
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task FilterQuizBank_ShouldReturnCorrectValue()
        {
            // Setup
            FilterQuizModel filter = new FilterQuizModel()
            {
                QuizTopic = new List<Guid>(),
                QuizType = new List<int>()
            };
            FilterQuizModel filter_badrequest = new FilterQuizModel()
            {
                QuizType = null,
                QuizTopic = null
            };
            var listName = new List<CreateQuizIntoBankDTO?>();
            _quizServiceMock.Setup(x => x.Filter(filter.QuizTopic, filter.QuizType)).ReturnsAsync(listName);
            _quizServiceMock.Setup(x => x.Filter(It.Is<List<Guid>>(x => x == null), It.Is<List<int>>(x => x == null))).ReturnsAsync(null as List<CreateQuizIntoBankDTO?>);

            // Act
            var result = await _testController.FilterQuizBank(filter);
            var result_badRequest = await _testController.FilterQuizBank(filter_badrequest);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(listName);
            result_badRequest.Should().BeOfType<BadRequestObjectResult>();
        }
        [Fact]
        public async Task ViewQuiz_ShouldReturnCorrectValue()
        {
            // Setup
            var QuizView = new DoingQuizDTO();
            _quizServiceMock.Setup(x => x.ViewDoingQuiz(It.IsAny<Guid>())).ReturnsAsync(QuizView);
            _quizServiceMock.Setup(x => x.ViewDoingQuiz(It.Is<Guid>(x => x == Guid.Empty))).ReturnsAsync(null as DoingQuizDTO);

            // Act
            var result = await _testController.ViewQuiz(Guid.NewGuid());
            var result_badRequest = await _testController.ViewQuiz(Guid.Empty);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(QuizView);
            result_badRequest.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task DoingQuiz_ShouldReturnCorrectValue()
        {
            // Setup
            var doingQuizDTOs = new List<AnswerQuizQuestionDTO>();
            var quizDtos = new AnswerQuizQuestionDTO()
            {
                QuizID = new Guid()
            };
            doingQuizDTOs.Add(quizDtos);
            var mark = 10;
            _quizServiceMock.Setup(x => x.DoingQuizService(doingQuizDTOs)).ReturnsAsync(true);
            _quizServiceMock.Setup(x => x.MarkQuiz(quizDtos.QuizID, It.IsAny<Guid>())).ReturnsAsync(mark);
            // Act
            var result = await _testController.DoingQuiz(doingQuizDTOs, Guid.NewGuid());

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo($"{mark:F2}");
        }
        [Fact]
        public async Task ViewDetailResult_ShouldReturnCorrectValue()
        {
            // Setup
            var markDetails = new List<ViewDetailResultDTO>();
            _quizServiceMock.Setup(x => x.ViewMarkDetail(It.IsAny<Guid>())).ReturnsAsync(markDetails);
            // Act
            var result = await _testController.ViewQuizDetail(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().Be(markDetails);
        }
        [Fact]
        public async Task ViewDetailResultByTrainee_ShouldReturnCorrectValue()
        {
            var result = await _testController.ViewDetailResultByTrainee(Guid.NewGuid());
            result.Should().BeOfType<OkObjectResult>();
        }
        [Fact]
        public async Task RemoveQuestionInBank_ShouldReturnCorrectValue()
        {
            _quizServiceMock.Setup(x => x.RemoveQuestionInBank(It.IsAny<Guid>())).ReturnsAsync(true);
            var result = await _testController.RemoveQuestionInBank(Guid.NewGuid());
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task RemoveQuestionInBank_ShouldReturnBadRequest()
        {
            var result = await _testController.RemoveQuestionInBank(Guid.NewGuid());
            result.Should().BeOfType<BadRequestResult>();
        }
        [Fact]
        public async Task UpdateQuestion_ShouldReturnCorrectValue()
        {
            UpdateQuestionDTO createQuizIntoBankDTO = _fixture.Create<UpdateQuestionDTO>();
            _quizServiceMock.Setup(x => x.UpdateQuestion(It.IsAny<Guid>(), createQuizIntoBankDTO)).ReturnsAsync(true);
            var result = await _testController.UpdateQuestion(Guid.NewGuid(), createQuizIntoBankDTO);
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task UpdateQuestion_ShouldReturnBadRequest()
        {
            UpdateQuestionDTO createQuizIntoBankDTO = _fixture.Create<UpdateQuestionDTO>();

            var result = await _testController.UpdateQuestion(Guid.NewGuid(), createQuizIntoBankDTO);
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
