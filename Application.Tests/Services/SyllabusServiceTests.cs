using Application.Interfaces;
using Application.Services;
using Application.ViewModels.SyllabusModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class SyllabusServiceTests : SetupTest
    {
        private readonly ISyllabusService _syllabusService;
        public SyllabusServiceTests()
        {
            _syllabusService = new SyllabusService(_unitOfWorkMock.Object, _claimsServiceMock.Object, _mapperConfig);
        }

        [Fact]
        public async void AddSyllabusAsync_ShouldThrowError()
        {
            //Setup
            var mockData = _fixture.Build<SyllabusGeneralDTO>().Create();
            //Act
            var result = async () => await _syllabusService.AddSyllabusAsync(mockData);
            // Assert

            await result.Should().ThrowAsync<AuthenticationException>();
        }
        [Fact]
        public async void AddSyllabusAsync_ReturnCorrectValue()
        {
            // Setup
            var mockData = _fixture.Build<SyllabusGeneralDTO>().Create();
            var mockUser = _fixture.Build<User>().Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Without(x => x.SubmitQuizzes).Create();
            var newSyllabus = new Syllabus();
            _mapperConfig.Map(mockData, newSyllabus);
            _claimsServiceMock.Setup(c => c.GetCurrentUserId).Returns(mockUser.Id);
            _unitOfWorkMock.Setup(c => c.SyllabusRepository).Returns(_syllabusRepositoryMock.Object);
            _syllabusRepositoryMock.Setup(a => a.AddSyllabusAsync(mockData)).ReturnsAsync(newSyllabus);
            // Act
            var result = await _syllabusService.AddSyllabusAsync(mockData);
            // Assert

            result.Should().NotBeNull();
        }
    }
}
