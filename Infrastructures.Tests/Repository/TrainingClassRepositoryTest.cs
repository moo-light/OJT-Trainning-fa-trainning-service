using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Domain.Enums;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class TrainingClassRepositoryTest : SetupTest
    {
        private readonly ITrainingClassRepository _trainingClassRepository;

        public TrainingClassRepositoryTest()
        {
            _trainingClassRepository = new TrainingClassRepository(
                _dbContext,
                _currentTimeMock.Object,
                _claimsServiceMock.Object);
        }

        [Fact]
        public async Task TrainingClassRepository_GetByIdAsync_ShouldReturnCorrectData()
        {
            var admin = new List<TrainingClassAdmin>();
            var trainer = new List<TrainingClassTrainer>();
            var mockData = _fixture.Build<TrainingClass>()
                .Without(x => x.TrainingClassParticipates)
                .Without(x => x.Applications)
                .Without(x => x.Attendances)
                .Without(x => x.Feedbacks)
                .Without(x => x.TrainingProgram)
                .Without(x => x.Location)
                .Without(x => x.ClassSchedules)
                .With(x => x.TrainingClassAdmins, admin)
                .With(x => x.TrainingClassTrainers, trainer)
                .Without(x => x.TrainingClassTimeFrame)
                .Without(x => x.TrainingClassAttendee)
                .With(x => x.IsDeleted, false).Create();

            await _dbContext.TrainingClasses.AddAsync(mockData);

            await _dbContext.SaveChangesAsync();

            var result = await _trainingClassRepository.GetByIdAsync(mockData.Id);

            result.Should().BeEquivalentTo(mockData);
        }


        [Fact]
        public async Task TrainingClassRepository_GetByIdAsync_ShouldReturnEmptyWhenHaveNoData()
        {

            var result = await _trainingClassRepository.GetByIdAsync(Guid.Empty);

            result.Should().BeNull();
        }

        [Fact]
        public async Task TrainingClassRepository_AddAsync_ShouldReturnCorrectData()
        {
            //arrange
            var mockData = _fixture.Build<TrainingClass>()
                .Without(x => x.TrainingClassParticipates)
                .Without(x => x.Applications)
                .Without(x => x.Attendances)
                .Without(x => x.Feedbacks)
                .Without(x => x.TrainingProgram)
                .Without(x => x.Location)
                .Without(x => x.ClassSchedules)
                .With(x => x.TrainingClassAdmins)//3
                .With(x => x.TrainingClassTrainers)//3
                .With(x => x.TrainingClassTimeFrame)//1+3
                .With(x => x.TrainingClassAttendee)//1
                .Create();//1
            //Act
            await _trainingClassRepository.AddAsync(mockData);
            var result = await _dbContext.SaveChangesAsync();
            //assert
            result.Should().Be(12);
        }

        [Fact]
        public async Task TrainingClassRepository_Update_ShouldReturnCorrectData()
        {
            //arrange
            var mockData = _fixture.Build<TrainingClass>()
                .OmitAutoProperties()
                .With(x => x.Code)
                .With(x => x.Name)
                .With(x => x.StatusClassDetail)
                .With(x => x.TrainingClassAdmins)
                .With(x => x.TrainingClassTrainers)
                .With(x => x.TrainingClassTimeFrame)
                .Create();
            //duplicate mock
            var mockAdmins = _fixture.Build<TrainingClassAdmin>()
                .OmitAutoProperties()
                .With(x => x.AdminId, mockData.TrainingClassAdmins.First().AdminId)
                .With(x => x.TrainingClassId, mockData.TrainingClassAdmins.First().TrainingClassId)
                .Create();
            var mockTrainers = _fixture.Build<TrainingClassTrainer>()
                .OmitAutoProperties()
                .With(x => x.TrainerId, mockData.TrainingClassTrainers.First().TrainerId)
                .With(x => x.TrainingClassId, mockData.TrainingClassTrainers.First().TrainingClassId)
                .Create();
            var mockHighlightDates = _fixture.Build<HighlightedDates>()
                .OmitAutoProperties()
                .With(x => x.HighlightedDate, mockData.TrainingClassTimeFrame.HighlightedDates?.First().HighlightedDate)
                .With(x => x.TrainingClassTimeFrameId, mockData.TrainingClassTimeFrame.Id)
                .Create();
            //act
            await _dbContext.TrainingClasses.AddAsync(mockData);
            await _dbContext.SaveChangesAsync();

            mockData.TrainingClassAdmins.Add(mockAdmins);
            mockData.TrainingClassTrainers.Add(mockTrainers);
            mockData.TrainingClassTimeFrame.HighlightedDates.Add(mockHighlightDates);

            _trainingClassRepository.Update(mockData);
            var result = await _dbContext.SaveChangesAsync();
            //assert
            result.Should().Be(1);
        }
    }
}
