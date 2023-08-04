using Application.Interfaces;
using Application.Repositories;
using Application.Services;
using FluentValidation.AspNetCore;
using Infrastructures.Repositories;
using Application.Utils;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI.Middlewares;
using WebAPI.Services;
using WebAPI.Controllers;
using Infrastructures;
using Hangfire;
using Application.Commons;
using Microsoft.AspNetCore.Http.Features;

namespace WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWebAPIService(this IServiceCollection services, string secretKey)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHealthChecks();
            services.AddSingleton<PerformanceMiddleware>();
            services.AddSingleton<Stopwatch>();
            services.AddScoped<ILectureService, LectureService>();
            services.AddScoped<IExternalAuthUtils, ExternalAuthUtils>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISyllabusService, SyllabusService>();
            services.AddScoped<IUnitService, UnitService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISyllabusRepository, SyllabusRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<ISyllabusService, SyllabusService>();
            services.AddScoped<ITrainingMaterialRepository, TrainingMaterialRepository>();
            services.AddScoped<ITrainingMaterialService, TrainingMaterialService>();
            services.AddSingleton<ExternalAuthUtils>();
            services.AddScoped<ITrainingClassService, TrainingClassService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<ITrainingProgramRepository, TrainingProgramRepository>();
            services.AddScoped<ITrainingProgramService, TrainingProgramService>();
            services.AddScoped<IDetailTrainingProgramSyllabusRepository, DetailTrainingProgramSyllabusRepository>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<IDetailQuizQuestionRepository, DetailQuizQuestionRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<IGradingService, GradingService>();
            services.AddScoped<ISubmitQuizRepository, SubmitQuizRepository>();
            services.AddScoped<IGradingRepository, GradingRepository>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IDetailTrainingClassParticipateRepository, DetailTrainingClassParticipateRepository>();
            services.AddScoped<IDetailTrainingClassParticipateService, DetailTrainingClassParticipateService>();
            services.AddScoped<ApplicationCronJob, ApplicationCronJob>();
            services.AddHttpContextAccessor();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            services.AddMemoryCache();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = secretKey,
                        ValidAudience = secretKey,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.FromSeconds(1)
                    };
                });

            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IAssignmentSubmisstionService, AssignmentSubmissionService>();

            ///HangFire
            services.AddHangfire(config => config
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseInMemoryStorage()
                 );
            services.AddHangfireServer();

            services.Configure<FormOptions>(o =>
             {
                 o.ValueLengthLimit = int.MaxValue;
                 o.MultipartBodyLengthLimit = int.MaxValue;
                 o.MemoryBufferThreshold = int.MaxValue;
             });

            return services;
        }
    }
}
