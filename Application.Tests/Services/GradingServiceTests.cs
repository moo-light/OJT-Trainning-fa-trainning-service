using Application.Interfaces;
using Application.Services;
using Application.ViewModels.GradingModels;
using Application.ViewModels.QuizModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class GradingServiceTests : SetupTest
    {
        private readonly IGradingService _gradingService;
        private readonly Guid id= Guid.NewGuid();
        public GradingServiceTests()
        {
            //_gradingService = new GradingService(_unitOfWorkMock.Object,_mapperConfig,_currentTimeMock.Object,_appConfigurationMock.Object);
            _gradingService = new GradingService(_unitOfWorkMock.Object, _mapperConfig, _currentTimeMock.Object, _appConfigurationMock.Object, _claimsServiceMock.Object);

        }

        [Fact]
        public async Task AddToGrading_ShouldBeTrue()
        {
            var checkLecture = _fixture.Build<Lecture>()
                .Without(x => x.Assignments)
                .Without(x => x.DetailUnitLectures)
                .Without(x => x.TrainingMaterials)
                .Without(x => x.AuditPlans)
                .Without(x => x.Gradings)
                .Without(x => x.Quiz)
                .Create();
            var grading = _fixture.Build<Grading>().Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.Lecture)
                .Create();
            var gradingMapper = _mapperConfig.Map<GradingModel>(grading);
            _unitOfWorkMock.Setup(x => x.LectureRepository.GetByIdAsync(grading.LectureId)).ReturnsAsync(checkLecture);

            var detailTrainingClass = _fixture.Build<DetailTrainingClassParticipate>()
                .Without(x => x.TrainingClass)
                //.Without(x => x.LocationName)
                .Without(x => x.User)
                .Create();
            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipate.GetByIdAsync(grading.DetailTrainingClassParticipateId))
                .ReturnsAsync(detailTrainingClass);

            _unitOfWorkMock.Setup(x => x.GradingRepository.AddAsync(grading)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var actualResult = await _gradingService.AddToGrading(gradingMapper);


            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task AddToGrading_ShouldBeFalse_checkDetailTrainingNull()
        {
            var checkLecture = new Lecture();
            var grading = _fixture.Build<Grading>().Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.Lecture)
                .Create();
            var gradingMapper = _mapperConfig.Map<GradingModel>(grading);
            _unitOfWorkMock.Setup(x => x.LectureRepository.GetByIdAsync(grading.LectureId)).ReturnsAsync(checkLecture);

            var detailTrainingClass = _fixture.Build<DetailTrainingClassParticipate>()
                .Without(x => x.TrainingClass)
                // .Without(x => x.LocationName)
                .Without(x => x.User)
                .Create();
            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipate.GetByIdAsync(grading.DetailTrainingClassParticipateId))
                .ReturnsAsync(detailTrainingClass = null);


            var actualResult = await _gradingService.AddToGrading(gradingMapper);


            actualResult.Should().BeFalse();
        }

        [Fact]
        public async Task AddToGrading_ShouldBeFalse_checkLectureNull()
        {
            var checkLecture = new Lecture();
            var grading = _fixture.Build<Grading>().Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.Lecture)
                .Create();
            var gradingMapper = _mapperConfig.Map<GradingModel>(grading);
            _unitOfWorkMock.Setup(x => x.LectureRepository.GetByIdAsync(grading.LectureId)).ReturnsAsync(checkLecture = null);

            var detailTrainingClass = _fixture.Build<DetailTrainingClassParticipate>()
                .Without(x => x.TrainingClass)
                //.Without(x => x.LocationName)
                .Without(x => x.User)
                .Create();
            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipate.GetByIdAsync(grading.DetailTrainingClassParticipateId))
                .ReturnsAsync(detailTrainingClass);

            var actualResult = await _gradingService.AddToGrading(gradingMapper);


            actualResult.Should().BeFalse();
        }

        [Fact]
        public async Task AddToGrading_ShouldBeFalse_SaveChange0()
        {
            var checkLecture = new Lecture();
            var grading = _fixture.Build<Grading>().Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.Lecture)
                .Create();
            var gradingMapper = _mapperConfig.Map<GradingModel>(grading);
            _unitOfWorkMock.Setup(x => x.LectureRepository.GetByIdAsync(grading.LectureId)).ReturnsAsync(checkLecture);

            var detailTrainingClass = _fixture.Build<DetailTrainingClassParticipate>()
                .Without(x => x.TrainingClass)
                //.Without(x => x.LocationName)
                .Without(x => x.User)
                .Create();
            _unitOfWorkMock.Setup(x => x.DetailTrainingClassParticipate.GetByIdAsync(grading.DetailTrainingClassParticipateId))
                .ReturnsAsync(detailTrainingClass);

            _unitOfWorkMock.Setup(x => x.GradingRepository.AddAsync(grading)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

            var actualResult = await _gradingService.AddToGrading(gradingMapper);

            actualResult.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateGradingReport()
        {
            _unitOfWorkMock.Setup(x => x.GradingRepository.UpdateGradingReports()).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();
            await _gradingService.UpdateGradingReports();
            _unitOfWorkMock.Verify(x => x.GradingRepository.UpdateGradingReports(),Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(),Times.Once);

        }
        [Fact]
        public async Task ViewAllQuizMark()
        {
            var fakeViewQuizAndMarkBelowDTO = _fixture.Build<ViewQuizAndMarkBelowDTO>().CreateMany(3).ToList();
            var mockUser = _fixture.Build<User>().Without(x => x.SubmitQuizzes).Without(x => x.Applications).Without(x => x.Syllabuses).Without(x => x.Feedbacks).Without(x => x.Attendances).Without(x => x.DetailTrainingClassParticipate).Create();
            _claimsServiceMock.Setup(s => s.GetCurrentUserId).Returns(Guid.Empty);
            _unitOfWorkMock.Setup(u=>u.UserRepository.GetByIdAsync(Guid.Empty)).ReturnsAsync(mockUser);
            _unitOfWorkMock.Setup(u => u.GradingRepository.GetAllMarkOfTrainee(mockUser.Id)).Returns(fakeViewQuizAndMarkBelowDTO);
            var actualResult = await _gradingService.ViewAllQuizMark();
            actualResult.Should(). Equal(fakeViewQuizAndMarkBelowDTO);
        }

        [Fact]
        public async Task ViewMarkQuizByQuizID()
        {
            var fakeListGrading = _fixture.Build<Grading>().Without(x => x.Lecture).Without(x => x.DetailTrainingClassParticipate).CreateMany(3).ToList();
            _unitOfWorkMock.Setup(u => u.GradingRepository.GetAllAsync()).ReturnsAsync(fakeListGrading);
            var actualResult = await _gradingService.ViewMarkQuizByQuizID(fakeListGrading.First().LectureId);
            actualResult.Should().Be((double)fakeListGrading.First().NumericGrade);
        }

        [Fact]
        public async Task UpdateGradingAsync_ShouldReturnFalse_WhenGradingisNull()
        {
            var gradingMock= _fixture.Build<Grading>()
                                     .Without(x=>x.DetailTrainingClassParticipate)
                                     .Without(x=>x.Lecture)
                                     .Create();
            _unitOfWorkMock.Setup(x => x.GradingRepository.GetByIdAsync(id)).ReturnsAsync(gradingMock = null);

            var actualResult = await _gradingService.UpdateGradingAsync(id, new GradingModel(Guid.NewGuid(), Guid.NewGuid(),"Ten",10));

            actualResult.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateGradingAsync_ShouldReturnTrue_WhenGradingisNotNull()
        {
            var gradingMock = _fixture.Build<Grading>()
                                     .Without(x => x.DetailTrainingClassParticipate)
                                     .Without(x => x.Lecture)
                                     .Create();
            var mapGradingMock = _mapperConfig.Map<GradingModel>(gradingMock);
            _unitOfWorkMock.Setup(x => x.GradingRepository.GetByIdAsync(id)).ReturnsAsync(gradingMock);
            mapGradingMock.LectureId = Guid.NewGuid();
            mapGradingMock.DetailTrainingClassParticipateId = Guid.NewGuid();
            mapGradingMock.LetterGrade = "Ten";
            mapGradingMock.NumericGrade = 10;
            _unitOfWorkMock.Setup(x => x.GradingRepository.Update(gradingMock)).Verifiable();
            
            var actualResult = await _gradingService.UpdateGradingAsync(id, mapGradingMock);

            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task GetMarkReportOfTrainee_ShouldReturnResult()
        {
            var listMarkReport = _fixture.Build<MarkReportDto>().CreateMany(1).ToList();
            _unitOfWorkMock.Setup(x => x.GradingRepository.GetMarkReportOfTrainee(id)).Returns(listMarkReport);

            var actualResult =  _gradingService.GetMarkReportOfTrainee(id);

            actualResult.Should().BeOfType<List<MarkReportDto>>();
        }

        [Fact]
        public async Task GetMarkReportOfClass_ShouldReturnResult()
        {
            var listMarkReport = _fixture.Build<MarkReportDto>().CreateMany(1).ToList();
            _unitOfWorkMock.Setup(x => x.GradingRepository.GetMarkReportOfClass(id)).Returns(listMarkReport);

            var actualResult = _gradingService.GetMarkReportOfClass(id);

            actualResult.Should().BeOfType<List<MarkReportDto>>();
        }

        [Fact]
        public async Task GetGradingAsync_ShouldReturnGrading()
        {
             var gradingMock=_fixture.Build<Grading>()
                                     .Without(x=>x.DetailTrainingClassParticipate)
                                     .Without(x=>x.Lecture)
                                     .Create();
            _unitOfWorkMock.Setup(x=>x.GradingRepository.GetByIdAsync(id)).ReturnsAsync(gradingMock);

            var actualResult= await _gradingService.GetGradingsAsync(id);

            actualResult.Should().BeOfType<Grading>();
        }

        [Fact]
        public async Task GetAllGradingsAsync()
        {
            var gradingMock = _fixture.Build<Grading>()
                                    .Without(x => x.DetailTrainingClassParticipate)
                                    .Without(x => x.Lecture)
                                    .CreateMany(2).ToList();
            _unitOfWorkMock.Setup(x=>x.GradingRepository.GetAllAsync()).ReturnsAsync(gradingMock);
            var actualResult = await _gradingService.GetAllGradingsAsync();
            actualResult.Should().Equal(gradingMock);
        }

        [Fact]
        public async Task DeleteGradingAsync_ReturnTrue()
        {
            var gradingMock = _fixture.Build<Grading>()
                                    .Without(x => x.DetailTrainingClassParticipate)
                                    .Without(x => x.Lecture)
                                    .Create();
            _unitOfWorkMock.Setup(x => x.GradingRepository.GetByIdAsync(id)).ReturnsAsync(gradingMock);
            _unitOfWorkMock.Setup(x => x.GradingRepository.SoftRemove(gradingMock)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();
            var actualResult = await _gradingService.DeleteGradingAsync(id);
            actualResult.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.GradingRepository.SoftRemove(gradingMock), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        }
        [Fact]
        public async Task DeleteGradingAsync_ReturnFalse()
        {
            var gradingMock = _fixture.Build<Grading>()
                                    .Without(x => x.DetailTrainingClassParticipate)
                                    .Without(x => x.Lecture)
                                    .Create();
            _unitOfWorkMock.Setup(x => x.GradingRepository.GetByIdAsync(id)).ReturnsAsync(gradingMock=null);
            var actualResult = await _gradingService.DeleteGradingAsync(id);
            actualResult.Should().BeFalse();

        }

        [Fact]
        public async Task CreateGradingAsync()
        {
            var gradingModel= _fixture.Build<GradingModel>().Create();
            var gradingMapper = _mapperConfig.Map<Grading>(gradingModel);
            _unitOfWorkMock.Setup(x=>x.GradingRepository.AddAsync(gradingMapper)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).Verifiable();
            await _gradingService.CreateGradingAsync(gradingModel);
            _unitOfWorkMock.Verify(x => x.SaveChangeAsync(), Times.Once);
        }
    }
}
