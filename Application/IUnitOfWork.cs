using Application.Repositories;
using Infrastructures.Repositories;

namespace Application
{
    public interface IUnitOfWork
    {
        public ISyllabusRepository SyllabusRepository { get; }

        public IApplicationRepository ApplicationRepository { get; }

        public IUserRepository UserRepository { get; }
        public IUnitRepository UnitRepository { get; }
        public ILectureRepository LectureRepository { get; }
        public IDetailUnitLectureRepository DetailUnitLectureRepository { get; }

        public ITrainingMaterialRepository TrainingMaterialRepository { get; }
        public IAttendanceRepository AttendanceRepository { get; }
        public IFeedbackRepository FeedbackRepository { get; }
        public IAssignmentSubmissionRepository AssignmentSubmissionRepository { get; }
        public ITrainingClassRepository TrainingClassRepository { get; }
        public ILocationRepository LocationRepository { get; }
        public IDetailTrainingProgramSyllabusRepository DetailTrainingProgramSyllabusRepository { get; }
        public ITrainingProgramRepository TrainingProgramRepository { get; }
        public IAuditPlanRepository AuditPlanRepository { get; }
        public IAuditQuestionRepository AuditQuestionRepository { get; }
        public IDetailAuditQuestionRepository DetailAuditQuestionRepository { get; }




        public ISubmitQuizRepository SubmitQuizRepository { get; }
        public IQuestionRepository QuestionRepository { get; }
        public IQuizRepository QuizRepository { get; }
        public IDetailQuizQuestionRepository DetailQuizQuestionRepository { get; }
        public ITopicRepository TopicRepository { get; }

        public IGradingRepository GradingRepository { get; }
        public IAuditSubmissionRepository AuditSubmissionRepository { get; }
        public IDetailAuditSubmissionRepository DetailAuditSubmissionRepository { get; }
        public IAssignmentRepository AssignmentRepository { get; }

        public IDetailTrainingClassParticipateRepository DetailTrainingClassParticipate { get; }
        public Task<int> SaveChangeAsync();
        public IDetailTrainingClassParticipateRepository DetailTrainingClassParticipateRepository { get; }
    }
}
