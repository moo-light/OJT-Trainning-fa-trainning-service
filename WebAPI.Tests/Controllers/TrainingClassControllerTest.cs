using Application.ViewModels.TrainingClassModels;
using AutoFixture;
using AutoMapper;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.Tests.Controllers
{
    public class TrainingClassControllerTest : SetupTest
    {
        private readonly TrainingClassController _trainingClassController;

        public TrainingClassControllerTest()
        {
            _trainingClassController = new TrainingClassController(_trainingClassServiceMock.Object, _claimsServiceMock.Object, _mapperConfig);
        }

        [Fact]
        public async Task CreateTrainingClass_ShouldReturnOK_WhenSavedSucceed()
        {
            //arrange
            var mock = _fixture.Build<CreateTrainingClassDTO>().Without(x => x.Attendee).Without(x => x.TimeFrame).Create();

            var expected = _fixture.Build<TrainingClassViewModel>().Create();
            _trainingClassServiceMock.Setup(
                x => x.CreateTrainingClassAsync(It.IsAny<CreateTrainingClassDTO>()))
                .ReturnsAsync(expected);

            //act
            var result = await _trainingClassController.CreateTrainingClassAsync(mock);

            //assert
            _trainingClassServiceMock.Verify(x => x.CreateTrainingClassAsync(It.Is<CreateTrainingClassDTO>(x => x.Equals(mock))), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result);
            (result as OkObjectResult)!.Value.Should().Be(expected);
        }

        [Fact]
        public async Task CreateTrainingClass_ShouldReturnBadRequest_WhenSavedFail()
        {
            //arrange
            var mock = _fixture.Build<CreateTrainingClassDTO>().Without(x => x.Attendees).Without(x => x.TimeFrame).Create();

            TrainingClassViewModel expected = null!;
            _trainingClassServiceMock.Setup(
                x => x.CreateTrainingClassAsync(It.IsAny<CreateTrainingClassDTO>()))
                .ReturnsAsync(expected);

            //act
            var result = await _trainingClassController.CreateTrainingClassAsync(mock);

            //assert
            _trainingClassServiceMock.Verify(x => x.CreateTrainingClassAsync(It.Is<CreateTrainingClassDTO>(x => x.Equals(mock))), Times.Once);
            Assert.IsType<BadRequestObjectResult>(result);
            (result as BadRequestObjectResult)!.Value.Should().BeEquivalentTo("Create training class fail: Saving fail");
        }
        [Fact]
        public async Task CreateTrainingClass_ShouldReturnBadRequest_WhenGetException()
        {
            //arrange
            var mock = _fixture.Build<CreateTrainingClassDTO>().Without(x => x.Attendees).Without(x => x.TimeFrame).Create();

            var exceptionMessage = "test message";

            _trainingClassServiceMock.Setup(
                x => x.CreateTrainingClassAsync(It.IsAny<CreateTrainingClassDTO>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            //act
            var result = await _trainingClassController.CreateTrainingClassAsync(mock);

            //assert
            _trainingClassServiceMock.Verify(x => x.CreateTrainingClassAsync(It.Is<CreateTrainingClassDTO>(x => x.Equals(mock))), Times.Once);
            Assert.IsType<BadRequestObjectResult>(result);
            (result as BadRequestObjectResult)!.Value.Should().Be("Create training class fail: " + exceptionMessage);
        }

        [Fact]
        public async Task CreateTrainingClassAsync_ShouldReturnCorrectData()
        {
            //arrange
            var mockCreate = _fixture.Build<CreateTrainingClassDTO2>().Create();
            TrainingClassViewModel expected = _fixture.Build<TrainingClassViewModel>().Create();
            _trainingClassServiceMock.Setup(
                x => x.CreateTrainingClassAsync(It.IsAny<CreateTrainingClassDTO>()))
                .ReturnsAsync(expected);
            //act
            var result = await _trainingClassController.CreateTrainingClassAsync(mockCreate);

            //assert
            result.Should().BeOfType<OkObjectResult>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task SoftRemoveTrainingClass_ShouldReturnNoContent_WhenSoftRemoveResultIsTrue()
        {
            //arrange
            var mockId = Guid.NewGuid();
            _trainingClassServiceMock
                .Setup(x => x.SoftRemoveTrainingClassAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            //act
            var result = await _trainingClassController.SoftRemoveTrainingClass(mockId.ToString());

            //assert
            _trainingClassServiceMock.Verify(x => x.SoftRemoveTrainingClassAsync(
                It.Is<string>(x => x.Equals(mockId.ToString()))
                ), Times.Once);
            Assert.IsType<NoContentResult>(result);
            //Assert.IsType<OkObjectResult>(result);
            //(result as OkObjectResult)!.Value.Should().Be("SoftRemove class successfully");
        }

        [Fact]
        public async Task SoftRemoveTrainingClass_ShouldReturnBadRequest_WhenSoftRemoveResultIsFalse()
        {
            //arrange
            var mockId = Guid.NewGuid();
            _trainingClassServiceMock
                .Setup(x => x.SoftRemoveTrainingClassAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            //act
            var result = await _trainingClassController.SoftRemoveTrainingClass(mockId.ToString());

            //assert
            _trainingClassServiceMock.Verify(x => x.SoftRemoveTrainingClassAsync(
                It.Is<string>(x => x.Equals(mockId.ToString()))
                ), Times.Once);
            Assert.IsType<BadRequestObjectResult>(result);
            (result as BadRequestObjectResult)!.Value.Should().Be("SoftRemove class fail: Saving fail");
        }
        [Fact]
        public async Task SoftRemoveTrainingClass_ShouldReturnBadRequest_WhenGetException()
        {
            //arrange
            var mockId = Guid.NewGuid();
            var exceptionMessage = "test message";
            _trainingClassServiceMock
                .Setup(x => x.SoftRemoveTrainingClassAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception(exceptionMessage));
            //act
            var result = await _trainingClassController.SoftRemoveTrainingClass(mockId.ToString());

            //assert
            _trainingClassServiceMock.Verify(x => x.SoftRemoveTrainingClassAsync(
                It.IsAny<string>()
                ), Times.Once);
            Assert.IsType<BadRequestObjectResult>(result);
            (result as BadRequestObjectResult)!.Value.Should().Be("SoftRemove class fail: " + exceptionMessage);
        }

        [Fact]
        public async Task UpdateTrainingClass_ShouldReturnNoContent_WhenUpdateResultIsTrue()
        {
            //arrange
            var mockId = Guid.NewGuid();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>().Create();
            _trainingClassServiceMock
                .Setup(x => x.UpdateTrainingClassAsync(It.IsAny<string>(), It.IsAny<UpdateTrainingClassDTO>()))
                .ReturnsAsync(true);

            //act
            var result = await _trainingClassController.UpdateTrainingClassAsync(mockId.ToString(), mockUpdate);

            //assert
            _trainingClassServiceMock.Verify(x => x.UpdateTrainingClassAsync(
                It.Is<string>(x => x.Equals(mockId.ToString())),
                It.Is<UpdateTrainingClassDTO>(x => x.Equals(mockUpdate))
                ), Times.Once);
            Assert.IsType<NoContentResult>(result);
            //Assert.IsType<OkObjectResult>(result);
            //(result as OkObjectResult)!.Value.Should().Be("Update class successfully");
        }

        [Fact]
        public async Task UpdateTrainingClass_ShouldReturnBadRequest_WhenUpdateResultIsFalse()
        {
            //arrange
            var mockId = Guid.NewGuid();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>().Create();
            _trainingClassServiceMock
                .Setup(x => x.UpdateTrainingClassAsync(It.IsAny<string>(), It.IsAny<UpdateTrainingClassDTO>()))
                .ReturnsAsync(false);

            //act
            var result = await _trainingClassController.UpdateTrainingClassAsync(mockId.ToString(), mockUpdate);

            //assert
            _trainingClassServiceMock.Verify(x => x.UpdateTrainingClassAsync(
                It.Is<string>(x => x.Equals(mockId.ToString())),
                It.Is<UpdateTrainingClassDTO>(x => x.Equals(mockUpdate))
                ), Times.Once);
            Assert.IsType<BadRequestObjectResult>(result);
            (result as BadRequestObjectResult)!.Value.Should().Be("Update class fail: Saving fail");
        }

        [Fact]
        public async Task UpdateTrainingClass_ShouldReturnBadRequest_WhenGetAutoMappingException()
        {
            //arrange
            var mockId = Guid.NewGuid();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO>().Create();
            var exceptionMessage = "test message";
            _trainingClassServiceMock
                .Setup(x => x.UpdateTrainingClassAsync(It.IsAny<string>(), It.IsAny<UpdateTrainingClassDTO>()))
                .ThrowsAsync(new Exception(exceptionMessage));
            //act
            var result = await _trainingClassController.UpdateTrainingClassAsync(mockId.ToString(), mockUpdate);

            //assert
            _trainingClassServiceMock.Verify(x => x.UpdateTrainingClassAsync(
                It.IsAny<string>(),
                It.IsAny<UpdateTrainingClassDTO>()
                ), Times.Once);
            Assert.IsType<BadRequestObjectResult>(result);
            (result as BadRequestObjectResult)!.Value.Should().Be("Update class fail: " + exceptionMessage);
        }

        [Fact]
        public async Task UpdateTrainingClassAsync_ShouldReturnCorrectData()
        {
            //arrange
            var mockId = Guid.NewGuid();
            var mockUpdate = _fixture.Build<UpdateTrainingClassDTO2>().Create();
            _trainingClassServiceMock
                .Setup(x => x.UpdateTrainingClassAsync(It.IsAny<string>(), It.IsAny<UpdateTrainingClassDTO>()))
                .ReturnsAsync(true);
            //act
            var result = await _trainingClassController.UpdateTrainingClassAsync(mockId.ToString(), mockUpdate);

            //assert
            result.Should().BeOfType<NoContentResult>();
            //result.Should().BeOfType<OkObjectResult>();
            //result.Should().NotBeNull();
        }

        [Fact]
        public async Task ViewDetailTrainingClass_ShouldOk_WhenFindSuccess()
        {
            //Arrange
            var mockData=_fixture.Create<FinalTrainingClassDTO>();
            var mockTrainingClass=_fixture.Build<TrainingClass>().
                                           OmitAutoProperties()
                                            .Without(x=>x.TrainingProgram)
                                            .Without(x=>x.TrainingClassParticipates)
                                            .Create<TrainingClass>();
            var existingClassID = mockTrainingClass.Id;
            _trainingClassServiceMock.Setup(s => s.GetFinalTrainingClassesAsync(existingClassID)).ReturnsAsync(mockData);
            //Act 
            var result =await  _trainingClassController.GetTrainingClassDetail(existingClassID);
            //Assert
            Assert.IsType<OkObjectResult>(result);
            
        }
       
        [Fact]
        public async Task ViewDetailTrainingClass_ShouldBeNotFound()
        {
            //Arrange
            var mockData = _fixture.Create<FinalTrainingClassDTO>();
            Guid mockId = Guid.NewGuid();
            //Act 
            // this shoule be return null
            var result_notFound = await _trainingClassController.GetTrainingClassDetail(mockId);
            //Assert
            Assert.IsType<NoContentResult>(result_notFound);
        }
        [Fact]
        public async Task ImportTrainingClass_ShouldOk_WhenImportSuccess()
        {
            //Arrange
            var mockData = _fixture.Build<TrainingClass>().OmitAutoProperties().CreateMany(2).ToList();
            Mock<IFormFile> file = new Mock<IFormFile>();
            //Act 
            // this shoule be return null

            _trainingClassServiceMock.Setup(s => s.ImportExcel(file.Object)).ReturnsAsync(mockData);// setup return mockData
            var result_ok = await _trainingClassController.ImportTrainingClass(file.Object);
            //Assert
            Assert.IsType<OkResult>(result_ok);
        }
        [Fact]
        public async Task ImportTrainingClass_ShouldBeBadRequest()
        {
            //Arrange
            Mock<IFormFile> file = new Mock<IFormFile>();

            //Act
            var result_badRequest = await _trainingClassController.ImportTrainingClass(file.Object);
            //Assert
            Assert.IsType<BadRequestResult>(result_badRequest);

        }
    }
}
