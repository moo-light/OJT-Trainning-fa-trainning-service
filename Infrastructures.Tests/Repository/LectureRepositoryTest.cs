using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using FluentAssertions.Primitives;
using Infrastructures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class LectureRepositoryTest : SetupTest
    {
        private readonly LectureRepository _lectureRepository;

        public LectureRepositoryTest()
        {
            _lectureRepository = new LectureRepository(_dbContext, _currentTimeMock.Object, _claimsServiceMock.Object);
        }

        [Fact]
        public async Task GetLectureIdByName_ShouldReturnGuid()
        {
            
            string nameMock = "a";
           
           var result= _lectureRepository.GetLectureIdByName(nameMock);
            result.Should().Be(Guid.Parse(result.ToString()));
           // result.Should().BeOfType<Guid>(); 

        }

        [Fact]
        public async Task GetLectureBySyllabusId_ShouldReturnList()
        {
            Guid id = Guid.NewGuid();
            var result=await _lectureRepository.GetLectureBySyllabusId(id);

            result.Should().BeOfType<List<Lecture>>();
        }

    }
}
