using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class LocationRepositoryTest : SetupTest
    {
        private readonly ILocationRepository _locationRepository;
        public LocationRepositoryTest()
        {
            _locationRepository = new LocationRepository(
                _dbContext,
                _currentTimeMock.Object,
                _claimsServiceMock.Object);
        }
        [Fact]
        public async Task LocationRepository_GetByNameAsync_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<Location>()
                .Without(x => x.TrainingClasses)
                .Without(x => x.DetailTrainingClassesParticipate)
                .With(x => x.LocationName)
                .With(x => x.IsDeleted, false)
                .Create();
            await _dbContext.Locations.AddAsync(mockData);

            await _dbContext.SaveChangesAsync();


            var result = await _locationRepository.GetByNameAsync(mockData.LocationName);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(mockData);
        }


        [Fact]
        public async Task LocationRepository_GetByNameAsync_ShouldReturnEmptyWhenHaveNoData()
        {

            var result = await _locationRepository.GetByNameAsync(It.IsAny<string>());

            result.Should().BeNull();
        }
    }
}
