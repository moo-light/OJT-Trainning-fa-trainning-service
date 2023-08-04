using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Infrastructures.FluentAPIs;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Metadata;

namespace Infrastructures
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {


        }

        #region DBSet
        public DbSet<User> Users { get; set; }
        public DbSet<Syllabus> Syllabuses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<DetailUnitLecture> DetailUnitLecture { get; set; }
        public DbSet<Unit> Units { get; set; }

        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<TrainingMaterial> TrainingMaterials { get; set; }
        public DbSet<TrainingClass> TrainingClasses { get; set; }
        public DbSet<TrainingClassAdmin> TrainingClassAdmins { get; set; }
        public DbSet<TrainingClassTrainer> TrainingClassTrainers { get; set; }
        public DbSet<TrainingClassAttendees> TrainingClassAttendees { get; set; }
        public DbSet<TrainingClassTimeFrame> TrainingClassTimeFrames { get; set; }
        public DbSet<HighlightedDates> HighlightedDates { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<DetailTrainingClassParticipate> DetailTrainingClassParticipates { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<TrainingProgram> TrainingPrograms { get; set; }
        public DbSet<AuditPlan> AuditPlans { get; set; }
        public DbSet<AuditSubmission> AuditSubmissions { get; set; }
        public DbSet<DetailAuditSubmission> DetailAuditSubmissions { get; set; }
        public DbSet<AuditQuestion> AuditQuestions { get; set; }
        public DbSet<DetailAuditQuestion> DetailAuditQuestions { get; set; }
        public DbSet<DetailTrainingProgramSyllabus> DetailTrainingProgramSyllabuses { get; set; }

        public DbSet<Topic> Topic { get; set; }

        public DbSet<DetailQuizQuestion> DetailQuizQuestions { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Quiz> Quizzes { get; set; }

        public DbSet<SubmitQuiz> SubmitQuiz { get; set; }

        public DbSet<Grading> Gradings { get; set; }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<GradingReport> GradingReports { get; set; }        
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
