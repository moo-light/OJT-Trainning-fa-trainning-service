using Application;
using Application.Utils;
using Application.Commons;
using Application.Interfaces;
using Application.Repositories;
using AutoFixture;
using AutoMapper;
using Infrastructures;
using Infrastructures.Mappers;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebAPI.Controllers;
using Microsoft.Extensions.Configuration;
using Application.Services;
using Infrastructures.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;
using WebAPI.Middlewares;

namespace Domains.Test
{
    public class SetupTest : IDisposable
    {

        protected readonly IMapper _mapperConfig;
        protected readonly Fixture _fixture;
        protected readonly Mock<IUnitOfWork> _unitOfWorkMock;

        protected readonly Mock<IUserRepository> _userRepositoryMock;
        protected readonly Mock<IUserService> _userServiceMock;

        protected readonly Mock<IUnitService> _unitServiceMock;
        protected readonly Mock<IUnitRepository> _unitRepositoryMock;
        protected readonly Mock<ITrainingMaterialRepository> _trainingMaterialRepositoryMock;
        protected readonly Mock<SyllabusController> _syllabusControllerMock;
        protected readonly Mock<ISyllabusService> _syllabusServiceMock;
        protected readonly Mock<ISyllabusRepository> _syllabusRepositoryMock;

        protected readonly Mock<IClaimsService> _claimsServiceMock;
        protected readonly Mock<ICurrentTime> _currentTimeMock;

        protected readonly Mock<IAttendanceService> _attendanceServiceMock;
        protected readonly Mock<IAttendanceRepository> _attendanceRepositoryMock;

        protected readonly Mock<IApplicationRepository> _applicationRepositoryMock;
        protected readonly Mock<IApplicationService> _applicationServiceMock;

        protected readonly Mock<IQuestionRepository> _questionRepositoryMock;
        protected readonly Mock<IQuestionService> _quizServiceMock;


        protected readonly Mock<IUserRepository> _userRepository;

        protected readonly Mock<ITrainingClassRepository> _trainingClassRepositoryMock;
        protected readonly Mock<ITrainingClassService> _trainingClassServiceMock;
        protected readonly Mock<ILocationRepository> _locationRepositoryMock;
        protected readonly Mock<ILocationService> _locationServiceMock;

        protected readonly Mock<IFeedbackService> _feedbackServiceMock;
        protected readonly Mock<IFeedbackRepository> _feedbackRepositoryMock;

        protected readonly AppConfiguration _appConfiguration;
        protected readonly AppDbContext _dbContext;
        protected readonly Mock<AppConfiguration> _appConfigurationMock;
        protected readonly Mock<ISendMailHelper> _sendMailMock;
        protected readonly Mock<IConfiguration> _config;
        protected readonly Mock<IMapper> _mapperMock;

        protected readonly Mock<ITrainingProgramService> _trainingProgramServiceMock;
        protected readonly Mock<ITrainingProgramRepository> _trainingProgramRepositoryMock;

        protected readonly Mock<ILectureService> _lectureServiceMock;
        protected readonly Mock<ILectureRepository> _lectureRepositoryMock;
        protected readonly Mock<IAuditPlanService> _auditPlanServiceMock;

        protected readonly Mock<IAuditSubmissionService> _auditSubmissionServiceMock;
        protected readonly Mock<ILogger<GlobalExceptionMiddlewareV2>> _loggerException; 
        protected readonly Mock<IGradingService> _gradingServiceMock;
        protected readonly Mock<IGradingRepository> _gradingRepositoryMock;
        protected readonly Mock<IDetailUnitLectureRepository> _detailUnitLectureRepositoryMock;
        protected readonly Mock<IAssignmentService> _assigmentServiceMock;
        protected readonly Mock<IRecurringJobManager> _recurringJobManagerMock;
        protected Mock<IQuestionService> QuestionServiceMock => _quizServiceMock;

        protected Mock<IQuestionRepository> QuestionRepositoryMock => _questionRepositoryMock;

        protected readonly Mock<IAssignmentSubmisstionService> _assignmentSubmissionServiceMock;
        protected readonly Mock<IDetailTrainingClassParticipateService> _detailTrainingClassParticipateService;
        protected readonly Mock<IAssignmentRepository> _assignmentRepositoryMock;
        protected readonly Mock<IAssignmentSubmissionRepository> _assignmentSubmissionRepositoryMock;
        protected readonly Mock<IDetailTrainingProgramSyllabusRepository> _detailTrainingProgramSyllabusRepositoryMock;
        protected readonly Mock<IQuizRepository> _quizRepositoryMock;
        protected readonly Mock<IDetailQuizQuestionRepository> _detailQuizQuestionRepositoryMock;
        protected readonly Mock<ITopicRepository> _topicRepositoryMock;
        protected readonly Mock<IDetailTrainingClassParticipateRepository> _detailTrainingClassParticipateRepositoryMock;
        protected readonly Mock<ISubmitQuizRepository> _submitQuizRepositoryMock;
        protected readonly Mock<IAuditSubmissionRepository> _auditSubmissionRepositoryMock;
        protected readonly Mock<IDetailAuditSubmissionRepository> _detailAuditSubmissionRepositoryMock;
        protected readonly Mock<IAuditPlanRepository> _auditPlanRepositoryMock;
        protected readonly Mock<IAuditQuestionRepository> _auditQuestionRepositoryMock;
        protected readonly Mock<IDetailAuditQuestionRepository> _detailAuditQuestionRepositoryMock;
       
    
        public SetupTest()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapperConfigurationsProfile());
            });
            _mapperConfig = mappingConfig.CreateMapper();
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
    .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerException = new Mock<ILogger<GlobalExceptionMiddlewareV2>>();
            _syllabusControllerMock = new Mock<SyllabusController>();
            _syllabusServiceMock = new Mock<ISyllabusService>();
            _syllabusRepositoryMock = new Mock<ISyllabusRepository>();

            _attendanceServiceMock = new Mock<IAttendanceService>();
            _attendanceRepositoryMock = new Mock<IAttendanceRepository>();

            _applicationRepositoryMock = new Mock<IApplicationRepository>();
            _applicationServiceMock = new Mock<IApplicationService>();
            _unitRepositoryMock = new Mock<IUnitRepository>();
            _unitServiceMock = new Mock<IUnitService>();
            _lectureServiceMock = new Mock<ILectureService>();
            _lectureRepositoryMock = new Mock<ILectureRepository>();

            _quizServiceMock = new Mock<IQuestionService>();
            _questionRepositoryMock = new Mock<IQuestionRepository>();

            _userServiceMock = new Mock<IUserService>();
            _claimsServiceMock = new Mock<IClaimsService>();
            _currentTimeMock = new Mock<ICurrentTime>();
            _sendMailMock = new Mock<ISendMailHelper>();
            _detailTrainingClassParticipateService = new Mock<IDetailTrainingClassParticipateService>();
            _trainingClassRepositoryMock = new Mock<ITrainingClassRepository>();
            _trainingClassServiceMock = new Mock<ITrainingClassService>();
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _unitServiceMock = new Mock<IUnitService>();
            _syllabusServiceMock = new Mock<ISyllabusService>();
            _feedbackServiceMock = new Mock<IFeedbackService>();
            _feedbackRepositoryMock = new Mock<IFeedbackRepository>();
            _trainingMaterialRepositoryMock = new Mock<ITrainingMaterialRepository>();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new AppDbContext(options);
            _appConfiguration = new AppConfiguration();
            _currentTimeMock.Setup(x => x.GetCurrentTime()).Returns(DateTime.UtcNow);
            _claimsServiceMock.Setup(x => x.GetCurrentUserId).Returns(Guid.Empty);
            _appConfigurationMock = new Mock<AppConfiguration>();
            _config = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();
            _trainingProgramServiceMock = new Mock<ITrainingProgramService>();
            _trainingProgramRepositoryMock = new Mock<ITrainingProgramRepository>();
            _trainingClassRepositoryMock = new Mock<ITrainingClassRepository>();
            _trainingClassServiceMock = new Mock<ITrainingClassService>();

            _locationRepositoryMock = new Mock<ILocationRepository>();
            _locationServiceMock = new Mock<ILocationService>();
            _auditPlanServiceMock = new Mock<IAuditPlanService>();

            _gradingServiceMock = new Mock<IGradingService>();
            _auditPlanServiceMock = new Mock<IAuditPlanService>();
            _auditSubmissionServiceMock = new Mock<IAuditSubmissionService>();
            _gradingRepositoryMock = new Mock<IGradingRepository>();

            _assigmentServiceMock = new Mock<IAssignmentService>();
            _recurringJobManagerMock = new Mock<IRecurringJobManager>();

            _assignmentSubmissionServiceMock = new Mock<IAssignmentSubmisstionService>();

            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _assignmentSubmissionRepositoryMock = new Mock<IAssignmentSubmissionRepository>();
            _detailUnitLectureRepositoryMock = new Mock<IDetailUnitLectureRepository>();
            //_trainingProgramRepositoryMock = new Mock<ITrainingProgramRepository>();
            _detailTrainingProgramSyllabusRepositoryMock = new Mock<IDetailTrainingProgramSyllabusRepository>();
            _quizRepositoryMock = new Mock<IQuizRepository>();
            _detailQuizQuestionRepositoryMock = new Mock<IDetailQuizQuestionRepository>();
            _topicRepositoryMock = new Mock<ITopicRepository>();
            _detailTrainingClassParticipateRepositoryMock = new Mock<IDetailTrainingClassParticipateRepository>();
            _submitQuizRepositoryMock = new Mock<ISubmitQuizRepository>();
            _auditSubmissionRepositoryMock= new Mock<IAuditSubmissionRepository>();
            _detailAuditSubmissionRepositoryMock= new Mock<IDetailAuditSubmissionRepository>();
            _auditPlanRepositoryMock = new Mock<IAuditPlanRepository>();
            _auditQuestionRepositoryMock = new Mock<IAuditQuestionRepository>();
            _detailAuditQuestionRepositoryMock = new Mock<IDetailAuditQuestionRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
    }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
