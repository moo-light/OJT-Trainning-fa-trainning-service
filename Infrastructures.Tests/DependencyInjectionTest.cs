using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Application.Commons;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.Utils;
using Castle.Core.Configuration;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebAPI;

namespace Infrastructures.Tests
{
    public class DependencyInjectionTest : SetupTest
    {
        private readonly ServiceProvider _serviceProvider;
        public DependencyInjectionTest()
        {
            var service = new ServiceCollection();
            service.AddWebAPIService("Testing");
            service.AddInfrastructuresService("Testing");

            service.AddDbContext<AppDbContext>(
                option => option.UseInMemoryDatabase("test"));
            _serviceProvider = service.BuildServiceProvider();
        }

        [Fact]
        public void GetInfrastructureService_ShouldReturnCorrectService() 
        {

            var userRepositoryResolved = _serviceProvider.GetRequiredService<IUserRepository>();
            var unitRepositoryResolved = _serviceProvider.GetRequiredService<IUnitRepository>();
            var attendanceRepositoryResolved = _serviceProvider.GetRequiredService<IAttendanceRepository>();
            var syllabusRepositoryResolved = _serviceProvider.GetRequiredService<ISyllabusRepository>();
            var lectureRepositoryResolved = _serviceProvider.GetRequiredService<ILectureRepository>();
            var unitOfWorkResolved = _serviceProvider.GetRequiredService<IUnitOfWork>();
            var detailUnitLectureRepositoryResolved = _serviceProvider.GetRequiredService<IDetailUnitLectureRepository>();
            var trainingClassRepositoryResolved = _serviceProvider.GetRequiredService<ITrainingClassRepository>();
            var locationRepositoryResolved = _serviceProvider.GetRequiredService<ILocationRepository>();
            var currentTimeResolved = _serviceProvider.GetRequiredService<ICurrentTime>();
            var assignmentRepositoryResolved = _serviceProvider.GetRequiredService<IAssignmentRepository>();
            var assignmentSubmssionRepositoryResolved = _serviceProvider.GetRequiredService<IAssignmentSubmissionRepository>();
            var unitService = _serviceProvider.GetRequiredService<IUnitService>();
            //var attendanceServiceResolved = _serviceProvider.GetRequiredService<IAttendanceService>();
            //var applicationService = _serviceProvider.GetRequiredService<IApplicationService>();
            var trainingProgramRepository = _serviceProvider.GetRequiredService<ITrainingProgramRepository>();
            var trainingProgramService = _serviceProvider.GetRequiredService<ITrainingProgramService>();
            var detailTrainingProgramSyllabusRepository = _serviceProvider.GetRequiredService<IDetailTrainingProgramSyllabusRepository>();           
            var auditPlanRepository = _serviceProvider.GetRequiredService<IAuditPlanRepository>();
            var auditQuestionRepository = _serviceProvider.GetRequiredService<IAuditQuestionRepository>();
            var detailAuditQuestionRepository = _serviceProvider.GetRequiredService<IDetailAuditQuestionRepository>();
            var auditPlanService = _serviceProvider.GetRequiredService<IAuditPlanService>();
            var feedBackRepository = _serviceProvider.GetRequiredService<IFeedbackRepository>();
           // var gradingService = _serviceProvider.GetRequiredService<IGradingService>();
            var gradingRepository = _serviceProvider.GetRequiredService<IGradingRepository>();
            //var questionService = _serviceProvider.GetRequiredService<IQuestionService>();
            var questionRepository = _serviceProvider.GetRequiredService<IQuestionRepository>();
            var quizRepository = _serviceProvider.GetRequiredService<IQuizRepository>();
            var detailQuizQuestionRepository = _serviceProvider.GetRequiredService<IDetailQuizQuestionRepository>();
            var topicRepository = _serviceProvider.GetRequiredService<ITopicRepository>();
            var detailTrainingClassParticipateRepository = _serviceProvider.GetRequiredService<IDetailTrainingClassParticipateRepository>();
            var auditSubmissionRepository = _serviceProvider.GetRequiredService<IAuditSubmissionRepository>();
            var detailAuditSubmissionRepository = _serviceProvider.GetRequiredService<IDetailAuditSubmissionRepository>();
            var auditSubmissionService = _serviceProvider.GetRequiredService<IAuditSubmissionService>();


/*
            sendMailHelperServiceResolved.GetType().Should().Be(typeof(SendMailHelper));*/
            userRepositoryResolved.GetType().Should().Be(typeof(UserRepository));
            unitRepositoryResolved.GetType().Should().Be(typeof(UnitRepository));
            attendanceRepositoryResolved.GetType().Should().Be(typeof(AttendanceRepository));
            syllabusRepositoryResolved.GetType().Should().Be(typeof(SyllabusRepository));
            lectureRepositoryResolved.GetType().Should().Be(typeof(LectureRepository));
            unitOfWorkResolved.GetType().Should().Be(typeof(UnitOfWork));
            detailUnitLectureRepositoryResolved.GetType().Should().Be(typeof(DetailUnitLectureRepository));
            trainingClassRepositoryResolved.GetType().Should().Be(typeof(TrainingClassRepository));
            locationRepositoryResolved.GetType().Should().Be(typeof(LocationRepository));
            currentTimeResolved.GetType().Should().Be(typeof(CurrentTime));
            assignmentRepositoryResolved.GetType().Should().Be(typeof(AssignmentRepository));
            assignmentSubmssionRepositoryResolved.GetType().Should().Be(typeof(AssignmentSubmissionRepository));
            unitService.GetType().Should().Be(typeof(UnitService));
            //attendanceServiceResolved.GetType().Should().Be(typeof(AttendanceService));
            //applicationService.GetType().Should().Be(typeof(ApplicationService));
            trainingProgramRepository.GetType().Should().Be(typeof(TrainingProgramRepository));
            trainingProgramService.GetType().Should().Be(typeof(TrainingProgramService));
            detailTrainingProgramSyllabusRepository.GetType().Should().Be(typeof(DetailTrainingProgramSyllabusRepository));
            auditPlanRepository.GetType().Should().Be(typeof(AuditPlanRepository));
            auditQuestionRepository.GetType().Should().Be(typeof(AuditQuestionRepository));
            detailAuditQuestionRepository.GetType().Should().Be(typeof(DetailAuditQuestionRepository));
            auditPlanService.GetType().Should().Be(typeof(AuditPlanService));
            feedBackRepository.GetType().Should().Be(typeof(FeedbackRepository));
            //gradingService.GetType().Should().Be(typeof(GradingService));
            gradingRepository.GetType().Should().Be(typeof(GradingRepository));
            //questionService.GetType().Should().Be(typeof(QuestionService));
            questionRepository.GetType().Should().Be(typeof(QuestionRepository));
            quizRepository.GetType().Should().Be(typeof(QuizRepository));
            detailQuizQuestionRepository.GetType().Should().Be(typeof(DetailQuizQuestionRepository));
            topicRepository.GetType().Should().Be(typeof(TopicRepository));
            detailTrainingClassParticipateRepository.GetType().Should().Be(typeof(DetailTrainingClassParticipateRepository));
            auditSubmissionRepository.GetType().Should().Be(typeof(AuditSubmissionRepository));
            detailAuditSubmissionRepository.GetType().Should().Be(typeof(DetailAuditSubmissionRepository));
        }
    }
}