using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures;
using Infrastructures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests
{
    public class UnitOfWorkTest : SetupTest
    {
        private readonly IUnitOfWork unitOfWork;
        public UnitOfWorkTest()
        {
            unitOfWork = new UnitOfWork(
                                        _dbContext, 
                                        _userRepositoryMock.Object,
                                        _trainingMaterialRepositoryMock.Object,
                                        _syllabusRepositoryMock.Object,
                                        _unitRepositoryMock.Object,
                                        _lectureRepositoryMock.Object,
                                        _detailUnitLectureRepositoryMock.Object,
                                        _trainingClassRepositoryMock.Object,
                                        _locationRepositoryMock.Object,
                                        _feedbackRepositoryMock.Object,
                                        _trainingProgramRepositoryMock.Object,
                                        _detailTrainingProgramSyllabusRepositoryMock.Object,
                                        _attendanceRepositoryMock.Object,
                                        _applicationRepositoryMock.Object,
                                        _quizRepositoryMock.Object,
                                        _detailQuizQuestionRepositoryMock.Object,
                                        _topicRepositoryMock.Object,
                                        _questionRepositoryMock.Object,
                                        _gradingRepositoryMock.Object,
                                        _auditPlanRepositoryMock.Object,
                                        _auditQuestionRepositoryMock.Object,
                                        _detailAuditQuestionRepositoryMock.Object,
                                        _detailTrainingClassParticipateRepositoryMock.Object,
                                        _submitQuizRepositoryMock.Object,
                                        _auditSubmissionRepositoryMock.Object,
                                        _detailAuditSubmissionRepositoryMock.Object,
                                        _assignmentRepositoryMock.Object,
                                        _assignmentSubmissionRepositoryMock.Object) ;
                
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldReturnPositive()
        {
            var auditPlan = _fixture.Build<AuditPlan>()
                                    .Without(x => x.Lecture)
                                    .Without(x => x.AuditSubmissions)
                                    .Without(x => x.DetailAuditQuestions)
                                    .Create();
            _dbContext.AuditPlans.Add(auditPlan);
            var result = await unitOfWork.SaveChangeAsync();
            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async void GetRepository_ShouldReturnCorrectService()
        {
            var userRepo = unitOfWork.UserRepository;
            userRepo.Should().BeAssignableTo<IUserRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService1()
        {
            var userRepo = unitOfWork.TrainingMaterialRepository;
            userRepo.Should().BeAssignableTo<ITrainingMaterialRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService2()
        {
            var userRepo = unitOfWork.AttendanceRepository;
            userRepo.Should().BeAssignableTo<IAttendanceRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService3()
        {
            var userRepo = unitOfWork.SyllabusRepository;
            userRepo.Should().BeAssignableTo<ISyllabusRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService4()
        {
            var userRepo = unitOfWork.UnitRepository;
            userRepo.Should().BeAssignableTo<IUnitRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService5()
        {
            var userRepo = unitOfWork.LectureRepository;
            userRepo.Should().BeAssignableTo<ILectureRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService6()
        {
            var userRepo = unitOfWork.DetailUnitLectureRepository;
            userRepo.Should().BeAssignableTo<IDetailUnitLectureRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService7()
        {
            var userRepo = unitOfWork.ApplicationRepository;
            userRepo.Should().BeAssignableTo<IApplicationRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService8()
        {
            var userRepo = unitOfWork.AuditSubmissionRepository;
            userRepo.Should().BeAssignableTo<IAuditSubmissionRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService9()
        {
            var userRepo = unitOfWork.DetailAuditSubmissionRepository;
            userRepo.Should().BeAssignableTo<IDetailAuditSubmissionRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService10()
        {
            var userRepo = unitOfWork.FeedbackRepository;
            userRepo.Should().BeAssignableTo<IFeedbackRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService12()
        {
            var userRepo = unitOfWork.LocationRepository;
            userRepo.Should().BeAssignableTo<ILocationRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService13()
        {
            var userRepo = unitOfWork.TrainingProgramRepository;
            userRepo.Should().BeAssignableTo<ITrainingProgramRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService14()
        {
            var userRepo = unitOfWork.DetailTrainingProgramSyllabusRepository;
            userRepo.Should().BeAssignableTo<IDetailTrainingProgramSyllabusRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService15()
        {
            var userRepo = unitOfWork.AuditPlanRepository;
            userRepo.Should().BeAssignableTo<IAuditPlanRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService16()
        {
            var userRepo = unitOfWork.AuditQuestionRepository;
            userRepo.Should().BeAssignableTo<IAuditQuestionRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService17()
        {
            var userRepo = unitOfWork.DetailAuditQuestionRepository;
            userRepo.Should().BeAssignableTo<IDetailAuditQuestionRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService18()
        {
            var userRepo = unitOfWork.SubmitQuizRepository;
            userRepo.Should().BeAssignableTo<ISubmitQuizRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService19()
        {
            var userRepo = unitOfWork.QuestionRepository;
            userRepo.Should().BeAssignableTo<IQuestionRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService20()
        {
            var userRepo = unitOfWork.QuizRepository;
            userRepo.Should().BeAssignableTo<IQuizRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService21()
        {
            var userRepo = unitOfWork.DetailQuizQuestionRepository;
            userRepo.Should().BeAssignableTo<IDetailQuizQuestionRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService22()
        {
            var userRepo = unitOfWork.TopicRepository;
            userRepo.Should().BeAssignableTo<ITopicRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService23()
        {
            var userRepo = unitOfWork.GradingRepository;
            userRepo.Should().BeAssignableTo<IGradingRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService24()
        {
            var userRepo = unitOfWork.AssignmentRepository;
            userRepo.Should().BeAssignableTo<IAssignmentRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService25()
        {
            var userRepo = unitOfWork.AssignmentSubmissionRepository;
            userRepo.Should().BeAssignableTo<IAssignmentSubmissionRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService26()
        {
            var userRepo = unitOfWork.TrainingClassRepository;
            userRepo.Should().BeAssignableTo<ITrainingClassRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService27()
        {
            var userRepo = unitOfWork.DetailTrainingClassParticipateRepository;
            userRepo.Should().BeAssignableTo<IDetailTrainingClassParticipateRepository>();
        }
        [Fact]
        public async void GetRepository_ShouldReturnCorrectService28()
        {
            var userRepo = unitOfWork.DetailTrainingClassParticipate;
            userRepo.Should().BeAssignableTo<IDetailTrainingClassParticipateRepository>();
        }

    }
}
