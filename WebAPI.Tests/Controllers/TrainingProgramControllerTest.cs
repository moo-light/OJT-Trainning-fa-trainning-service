using Application.ViewModels.TrainingProgramModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class TrainingProgramControllerTest : SetupTest
    {
        private readonly TrainingProgramController trainingProgramController;
        public TrainingProgramControllerTest()
        {
            trainingProgramController = new TrainingProgramController(_trainingProgramServiceMock.Object);

        }

        [Fact]
        public async Task GetDetailTrainingProgram_ShouldReturnOk()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.TrainingClasses).Create<TrainingProgram>();
            var trainingProgramView = _mapperConfig.Map<TrainingProgramViewModel>(trainingProgram);
            var trainingProgramId = trainingProgram.Id;
            _trainingProgramServiceMock.Setup(tp => tp.GetTrainingProgramDetail(trainingProgramId)).ReturnsAsync(trainingProgramView);

            var result = await trainingProgramController.GetDetail(trainingProgram.Id);
            result.Should().BeOfType(typeof(OkObjectResult));


        }

        [Fact]
        public async Task GetDetailTrainingProgram_ShouldReturnBadRequest()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>().Without(x => x.DetailTrainingProgramSyllabus).Without(x => x.TrainingClasses).Create<TrainingProgram>();
            var trainingProgramId = trainingProgram.Id;
            var trainingProgramView = _mapperConfig.Map<TrainingProgramViewModel>(trainingProgram);
            _trainingProgramServiceMock.Setup(tp => tp.GetTrainingProgramDetail(trainingProgramId))!.ReturnsAsync(trainingProgramView = null);
            var result = await trainingProgramController.GetDetail(trainingProgram.Id);
            result.Should().BeOfType(typeof(BadRequestResult));

        }

        [Fact]
        public async Task CreateTrainingProgram_ShouldReturn201()
        {
            var createTrainingDTO = _fixture.Build<UpdateTrainingProgramDTO>().Create();
            var trainingProgram = _mapperConfig.Map<TrainingProgram>(createTrainingDTO);
            _trainingProgramServiceMock.Setup(tp => tp.CreateTrainingProgram(createTrainingDTO)).ReturnsAsync(trainingProgram);
            var result = await trainingProgramController.Create(createTrainingDTO);
            result.Should().BeAssignableTo<CreatedAtActionResult>();
        }

        [Fact]
        public async Task CreateTrainingProgram_ShouldReturn400()
        {
            var createTrainingDTO = _fixture.Build<UpdateTrainingProgramDTO>().Create();
            var trainingProgram = _mapperConfig.Map<TrainingProgram>(createTrainingDTO = null);
            var result = await trainingProgramController.Create(createTrainingDTO!);
            result.Should().BeAssignableTo<BadRequestResult>();
        }
        /*        [Fact]
                public async Task UpdateTrainingProgram_ShouldReturn204()
                {
                    var updateProgramDTO = _fixture.Build<UpdateTrainingProgramDTO>().Create();
                    _trainingProgramServiceMock.Setup(m => m.UpdateTrainingProgram(It.IsAny<UpdateTrainingProgramDTO>())).ReturnsAsync(true);

                    var actualResult = await trainingProgramController.Update(updateProgramDTO);
                    actualResult.Should().BeAssignableTo<NoContentResult>();
                }
        */
        /*        [Fact]
                public async Task UpdateTrainingProgram_ShouldReturn400()
                {
                    var updateProgramDTO = _fixture.Build<UpdateTrainingProgramDTO>().Create();
                    _trainingProgramServiceMock.Setup(m => m.UpdateTrainingProgram(It.IsAny<UpdateTrainingProgramDTO>())).ReturnsAsync(false);

                    var actualResult = await trainingProgramController.Update(updateProgramDTO);
                    actualResult.Should().BeAssignableTo<BadRequestResult>();
                }*/
        [Fact]
        public async Task SearchTrainingProgramWithFilter_ShouldReturnCorrectData()
        {
            //arrange
            var tmpGuid = Guid.NewGuid();
            var mockTPs = _fixture.Build<SearchAndFilterTrainingProgramViewModel>().With(u => u.CreatedBy, tmpGuid)
                                                                                .Without(u => u.Id)
                                                                                .Without(u => u.TrainingTitle)
                                                                                .Without(u => u.CreationDate)
                                                                                .Without(u => u.Durations)
                                                                    .CreateMany(3).ToList();
            _trainingProgramServiceMock.Setup(x => x.SearchTrainingProgramWithFilter(mockTPs[1].TrainingTitle, mockTPs[1].Status, mockTPs[1].CreatedBy.ToString())).ReturnsAsync(mockTPs);
            //act
            var result = await trainingProgramController.Search(mockTPs[1].TrainingTitle, mockTPs[1].Status, mockTPs[1].CreatedBy.ToString()) as OkObjectResult;
            //assert
            _trainingProgramServiceMock.Verify(x => x.SearchTrainingProgramWithFilter(mockTPs[1].TrainingTitle, mockTPs[1].Status, mockTPs[1].CreatedBy.ToString()), Times.Once);
            Assert.NotNull(result);
            Assert.IsType<List<SearchAndFilterTrainingProgramViewModel>>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(mockTPs, result.Value);
        }

        [Fact]
        public async Task SearchUserWithFilter_ShouldReturnNoContent_WhenIsNullOrEmpty()
        {
            //act
            var result = await trainingProgramController.Search("", "", "") as NoContentResult;
            //assert
            _trainingProgramServiceMock.Verify(x => x.SearchTrainingProgramWithFilter("", "", ""), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
        }

        [Fact]
        public async Task GetAllTrainingProgram_ShouldReturnOk()
        {
            var listTrainingProgram = _fixture.Build<List<ViewAllTrainingProgramDTO>>().Create();
            _trainingProgramServiceMock.Setup(x => x.ViewAllTrainingProgramDTOs()).ReturnsAsync(listTrainingProgram);
            var actualResult = await trainingProgramController.GetAllTrainingProgram();
            actualResult.Should().BeAssignableTo<OkObjectResult>();
        }
        [Fact]
        public async Task GetAllTrainingProgram_ShouldReturnBadRequest()
        {
            var listTrainingProgram = _fixture.Build<List<ViewAllTrainingProgramDTO>>().Create();
            _trainingProgramServiceMock.Setup(x => x.ViewAllTrainingProgramDTOs())!.ReturnsAsync(listTrainingProgram = null);
            var actualResult = await trainingProgramController.GetAllTrainingProgram();
            actualResult.Should().BeAssignableTo<BadRequestObjectResult>();
        }
        [Fact]
        public async Task DuplicateTrainingProgram_ShouldReturn201()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>()
                                 .Without(x => x.TrainingClasses)
                                 .Without(x => x.DetailTrainingProgramSyllabus)
                                 .Create();
            _trainingProgramServiceMock.Setup(x => x.DuplicateTrainingProgram(It.IsAny<Guid>())).ReturnsAsync(trainingProgram);
            var result = await trainingProgramController.Duplicate(Guid.NewGuid());
            result.Should().BeAssignableTo<CreatedAtActionResult>();
            
        }
        [Fact]
        public async Task DuplicateTrainingProgram_ShouldReturn400()
        {
            var trainingProgram = _fixture.Build<TrainingProgram>()
                                 .Without(x => x.TrainingClasses)
                                 .Without(x => x.DetailTrainingProgramSyllabus)
                                 .Create();
            _trainingProgramServiceMock.Setup(x => x.DuplicateTrainingProgram(It.IsAny<Guid>()))!.ReturnsAsync(trainingProgram = null);
            var result = await trainingProgramController.Duplicate(Guid.NewGuid());
            result.Should().BeAssignableTo<BadRequestObjectResult>();
        }
    }
}
