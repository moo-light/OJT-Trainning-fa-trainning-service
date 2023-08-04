using Application;
using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using Application.Utils;
using Domain.Entities;
using Infrastructures.Mappers;
using Infrastructures.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Infrastructures
{
    public static class DenpendencyInjection
    {
        public static IServiceCollection AddInfrastructuresService(this IServiceCollection services, string databaseConnection)
        {
            services.AddSingleton<ISendMailHelper, SendMailHelper>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitRepository, UnitRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddScoped<ISyllabusRepository, SyllabusRepository>();
            services.AddScoped<ILectureRepository, LectureRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDetailUnitLectureRepository, DetailUnitLectureRepository>();
            services.AddScoped<ITrainingClassRepository, TrainingClassRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddSingleton<ICurrentTime, CurrentTime>();
            services.AddScoped<IAssignmentRepository,AssignmentRepository>();
            services.AddScoped<IAssignmentSubmissionRepository, AssignmentSubmissionRepository>();

            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISyllabusService, SyllabusService>();
            services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            services.AddSingleton<ExternalAuthUtils>();
            services.AddScoped<IApplicationService, ApplicationService>();      
            services.AddScoped<ITrainingProgramRepository, TrainingProgramRepository>();
            services.AddScoped<ITrainingProgramService, TrainingProgramService>();
            services.AddScoped<IDetailTrainingProgramSyllabusRepository, DetailTrainingProgramSyllabusRepository>();       
            services.AddScoped<IAuditPlanRepository, AuditPlanRepository>();
            services.AddScoped<IAuditQuestionRepository, AuditQuestionRepository>();
            services.AddScoped<IDetailAuditQuestionRepository, DetailAuditQuestionRepository>();
            services.AddScoped<IAuditPlanService, AuditPlanService>();

            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IGradingService, GradingService>();
            services.AddScoped<IGradingRepository, GradingRepository>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<IDetailQuizQuestionRepository, DetailQuizQuestionRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();    
            services.AddScoped<IDetailTrainingClassParticipateRepository, DetailTrainingClassParticipateRepository>();
            services.AddScoped<IAuditSubmissionRepository, AuditSubmissionRepository>();
            services.AddScoped<IDetailAuditSubmissionRepository, DetailAuditSubmissionRepository>();
            services.AddScoped<IAuditSubmissionService, AuditSubmissionService>();
            // ATTENTION: if you do migration please check file README.md
            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(databaseConnection).EnableSensitiveDataLogging());
            // this configuration just use in-memory for fast develop
            //services.AddDbContext<AppDbContext>(option => option.UseInMemoryDatabase("test"));
            services.AddAutoMapper(typeof(MapperConfigurationsProfile).Assembly);
            return services;
        }
    }
}
