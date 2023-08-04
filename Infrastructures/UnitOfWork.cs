using Application;
using Application.Repositories;
using Infrastructures.Repositories;
using System.Runtime.CompilerServices;

namespace Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISyllabusRepository _syllabusRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ILectureRepository _lectureRepository;
        private readonly IDetailUnitLectureRepository _detailUnitLectureRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ITrainingMaterialRepository _trainingMaterialRepository;
        private readonly IAuditSubmissionRepository _auditSubmissionRepository;
        private readonly IDetailAuditSubmissionRepository _detailAuditSubmissionRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ITrainingClassRepository _trainingClassRepository;
        public readonly ILocationRepository _locationRepository;
        private readonly ITrainingProgramRepository _trainingProgramRepository;
        private readonly IDetailTrainingProgramSyllabusRepository _detailTrainingProgramSyllabusRepository;
        private readonly IAuditPlanRepository _auditPlanRepository;
        private readonly IAuditQuestionRepository _auditQuestionRepository;
        private readonly IDetailAuditQuestionRepository _detailAuditQuestionRepository;
        private readonly ISubmitQuizRepository _submitQuizRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IDetailQuizQuestionRepository _detailQuizQuestionRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly IDetailTrainingClassParticipateRepository _detailTrainingClassParticipateRepository;
        private readonly IGradingRepository _gradingRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssignmentSubmissionRepository _assignmentSubmissionRepository;
        public IDetailTrainingClassParticipateRepository _detailTrainingClassParticipate;
        public UnitOfWork(AppDbContext dbContext,
            IUserRepository userRepository, ITrainingMaterialRepository trainingMaterialRepository,
            ISyllabusRepository syllabusRepository, IUnitRepository unitRepository, ILectureRepository lectureRepository, IDetailUnitLectureRepository detailUnitLectureRepository, ITrainingClassRepository trainingClassRepository, ILocationRepository locationRepository,
            IFeedbackRepository feedbackRepository, ITrainingProgramRepository trainingProgramRepository, IDetailTrainingProgramSyllabusRepository detailTrainingProgramSyllabusRepository, IAttendanceRepository attendanceRepository, IApplicationRepository applicationReapository,
            IQuizRepository quizRepository, IDetailQuizQuestionRepository detailQuizQuestionRepository, ITopicRepository topicRepository, IQuestionRepository questionRepository,
            IGradingRepository gradingRepository,
            IAuditPlanRepository auditPlanRepository,
            IAuditQuestionRepository auditQuestionRepository,
            IDetailAuditQuestionRepository detailAuditQuestionRepository,
            IDetailTrainingClassParticipateRepository detailTrainingClassParticipateRepository, ISubmitQuizRepository submitQuizRepository,
            IAuditSubmissionRepository auditSubmissionRepository, IDetailAuditSubmissionRepository detailAuditSubmissionRepository,
             IAssignmentRepository assignmentRepository, IAssignmentSubmissionRepository assignmentSubmissionRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _syllabusRepository = syllabusRepository;
            _unitRepository = unitRepository;
            _lectureRepository = lectureRepository;
            _detailUnitLectureRepository = detailUnitLectureRepository;
            _trainingMaterialRepository = trainingMaterialRepository;
            _trainingClassRepository = trainingClassRepository;
            _locationRepository = locationRepository;
            _trainingProgramRepository = trainingProgramRepository;
            _detailTrainingProgramSyllabusRepository = detailTrainingProgramSyllabusRepository;
            _feedbackRepository = feedbackRepository;
            _attendanceRepository = attendanceRepository;
            _assignmentRepository = assignmentRepository;
            _assignmentSubmissionRepository = assignmentSubmissionRepository;
            _applicationRepository = applicationReapository;
            _quizRepository = quizRepository;
            _detailQuizQuestionRepository = detailQuizQuestionRepository;
            _topicRepository = topicRepository;
            _questionRepository = questionRepository;
            _gradingRepository = gradingRepository;
            _auditPlanRepository = auditPlanRepository;
            _auditQuestionRepository = auditQuestionRepository;
            _detailAuditQuestionRepository = detailAuditQuestionRepository;
            _submitQuizRepository = submitQuizRepository;
            _detailTrainingClassParticipateRepository = detailTrainingClassParticipateRepository;
            _auditSubmissionRepository = auditSubmissionRepository;
            _detailAuditSubmissionRepository = detailAuditSubmissionRepository;
            _auditPlanRepository = auditPlanRepository;
        }
        public IUserRepository UserRepository => _userRepository;
        public ISyllabusRepository SyllabusRepository => _syllabusRepository;
        public IUnitRepository UnitRepository => _unitRepository;
        public ILectureRepository LectureRepository => _lectureRepository;
        public IFeedbackRepository FeedbackRepository => _feedbackRepository;
        public IDetailUnitLectureRepository DetailUnitLectureRepository => _detailUnitLectureRepository;
        public ITrainingMaterialRepository TrainingMaterialRepository => _trainingMaterialRepository;
        public ITrainingProgramRepository TrainingProgramRepository => _trainingProgramRepository;
        public ITrainingClassRepository TrainingClassRepository => _trainingClassRepository;
        public ILocationRepository LocationRepository => _locationRepository;
        public IDetailTrainingProgramSyllabusRepository DetailTrainingProgramSyllabusRepository => _detailTrainingProgramSyllabusRepository;
        public IAssignmentRepository AssignmentRepository => _assignmentRepository;
        public IAttendanceRepository AttendanceRepository => _attendanceRepository;
        public IApplicationRepository ApplicationRepository => _applicationRepository;
        public IQuestionRepository QuestionRepository => _questionRepository;
        public IQuizRepository QuizRepository => _quizRepository;
        public IDetailQuizQuestionRepository DetailQuizQuestionRepository => _detailQuizQuestionRepository;
        public ITopicRepository TopicRepository => _topicRepository;
        public IGradingRepository GradingRepository => _gradingRepository;
        public IAuditPlanRepository AuditPlanRepository => _auditPlanRepository;
        public IAuditQuestionRepository AuditQuestionRepository => _auditQuestionRepository;
        public IDetailAuditQuestionRepository DetailAuditQuestionRepository => _detailAuditQuestionRepository;
        public IDetailTrainingClassParticipateRepository DetailTrainingClassParticipateRepository => _detailTrainingClassParticipateRepository;
        public IAssignmentSubmissionRepository AssignmentSubmissionRepository => _assignmentSubmissionRepository;
        public ISubmitQuizRepository SubmitQuizRepository => _submitQuizRepository;
        public IAuditSubmissionRepository AuditSubmissionRepository => _auditSubmissionRepository;
        public IDetailAuditSubmissionRepository DetailAuditSubmissionRepository => _detailAuditSubmissionRepository;
        public IDetailTrainingClassParticipateRepository DetailTrainingClassParticipate => _detailTrainingClassParticipateRepository;
        public async Task<int> SaveChangeAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}

