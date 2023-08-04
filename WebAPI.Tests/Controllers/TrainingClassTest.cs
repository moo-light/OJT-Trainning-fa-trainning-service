using Application;
using Application.Interfaces;
using Application.Services;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using Application.ViewModels.TrainingClassModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using FluentAssertions.Common;
using Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Writers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class TrainingClassTest : SetupTest
    {
        private readonly TrainingClassController _trainingClassController;

        public TrainingClassTest()
        {
            _trainingClassController = new TrainingClassController(_trainingClassServiceMock.Object, _claimsServiceMock.Object, _mapperConfig);

        }

        [Fact]
        public async Task SearchClassByName_Get_ShouldReturnCorrectValues()
        {
            List<TrainingClassViewAllDTO> trainingClasses = _fixture.Build<TrainingClassViewAllDTO>()
                                                          .OmitAutoProperties()
                                                          .With(x => x.classCode)
                                                          .With(x => x.className)
                                                          .With(x => x.fsu)
                                                          .With(x => x.createdBy)
                                                          .With(x => x.createdOn)
                                                          .With(x=>x.id)
                                                          .CreateMany(3)
                                                          .ToList();
          /*  await _dbContext.AddRangeAsync(trainingClasses);
            await _dbContext.SaveChangesAsync();*/

            string name1 = "anything";

            _trainingClassServiceMock.Setup(s => s.SearchClassByNameAsync(name1)).ReturnsAsync(trainingClasses);

            var result = await _trainingClassController.SearchClassByName(name1);
            result.Should().BeOfType<OkObjectResult>();


        }
        [Fact]
        public async Task DuplicateClass_ShouldReturnOkObjectResult_WhenValidIdIsProvided()
        {
            var existingClassId = Guid.NewGuid();
            var existingClass = _fixture.Build<TrainingClass>().
                OmitAutoProperties()
                .With(x => x.Name)
                .With(x => x.StartTime)
                .With(x => x.EndTime)
                .With(x => x.Code)
                .With(x => x.Duration)
                .With(x => x.Attendee)
                .With(x => x.Branch)
                .With(x => x.Id, existingClassId).Create();
            await _dbContext.SaveChangesAsync();

            _trainingClassServiceMock.Setup(s => s.DuplicateClassAsync(existingClassId)).ReturnsAsync(true);

            // Act
            var result = await _trainingClassController.DuplicateClass(existingClassId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetAllTraningClass_ReturnsOk_WhenListResultIsNotNull()
        {
            // Arrange
            List<TrainingClassViewAllDTO> trainingClasses = _fixture.CreateMany<TrainingClassViewAllDTO>(3).ToList();
            _trainingClassServiceMock.Setup(s => s.GetAllTrainingClassesAsync()).ReturnsAsync(trainingClasses);

            // Act
            var result = await _trainingClassController.GetAllTraningClass();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
      
        [Fact]
        public async Task FilterResult_ReturnsNoContentObject_WhenFilterResultIsEmpty()
        { 
            var mockData = _fixture.Build<TrainingClassFilterModel>().Create();
            _trainingClassServiceMock.Setup(x => x.FilterLocation(mockData.locationName, mockData.branchName, mockData.date1, mockData.date2, mockData.classStatus, mockData.attendInClass, mockData.trainer));
            //act
          var result=await _trainingClassController.FilterResult(mockData);
            //assert
            Assert.NotNull(result);
            result.Should().BeOfType<NoContentResult>();
        }
        [Fact]
        public async Task FilterResult_ReturnsOkObjectResult_WhenSearchIsNotEmpty()
        {
            // Arrange
            string[] locationName = { "Ftown2" };
            DateTime date1 = DateTime.Parse("2023/02/28");
            DateTime date2 = DateTime.Parse("2023/03/31");
            string[] statusClass = { "Active" };
            string[] attendee = { "50" };
            string branchName = "stringstring";
            string trainerName = "Name";
            var nonEmptyResult = _fixture.Build<TrainingClassViewAllDTO>()
                .CreateMany(10).ToList();
            var mockData = new TrainingClassFilterModel()
            {
                branchName = branchName,
                locationName = locationName,
                date1 = date1,
                date2 = date2,
                attendInClass = attendee,
                classStatus = statusClass,
                trainer = trainerName
            };
            _trainingClassServiceMock.Setup(x => x.FilterLocation(locationName, branchName, date1, date2, statusClass, attendee, trainerName)).ReturnsAsync(nonEmptyResult);

            // Act
            var result = await _trainingClassController.FilterResult(mockData);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<TrainingClassViewAllDTO>>(okObjectResult.Value);
        }
    }
}


