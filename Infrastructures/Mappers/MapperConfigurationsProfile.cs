using AutoMapper;
using Application.Commons;
using Domain.Entities;
using Application.ViewModels.UserViewModels;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.Location;
using Application.ViewModels.FeedbackModels;

using Application.ViewModels.ApplicationViewModels;
using Application.ViewModels.AtttendanceModels;
using Domain.Enums;
using Application.ViewModels.GradingModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.UpdateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels;
using Application.ViewModels.AuditModels.ViewModels;
using Application.ViewModels.FeedbackModels;
using Application.ViewModels.GradingModels;
using Application.ViewModels.Location;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.FixViewSyllabus;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingProgramModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using Application.ViewModels.UserViewModels;
using Domain.Entities;
using Domain.Entities.TrainingClassRelated;
using Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.Collections.Generic;
using Application.ViewModels.AuditModels;
using Application.ViewModels.AssignmentModel;
using Application.ViewModels.QuizModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using System.Security.Cryptography.Xml;
using Infrastructures.Mappers.TypeConverter;
using Infrastructures.Mappers.ValueConverters;

namespace Infrastructures.Mappers
{
    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            CreateMap(typeof(Pagination<>), typeof(Pagination<>));

            // Create Mapping UpdateDTO -- User
            CreateMap<UpdateDTO, User>()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            CreateMap<RegisterDTO, User>()
                .ForMember(rd => rd.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(rd => rd.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(rd => rd.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ReverseMap();

            CreateMap<AddUserManually, User>()
               .ForMember(destinationMember => destinationMember.UserName, options => options.MapFrom(src => src.UserName))
               .ForMember(destinationMember => destinationMember.PasswordHash, options => options.MapFrom(src => src.Pass))
              .ForMember(destinationMember => destinationMember.FullName, options => options.MapFrom(src => src.FullName))
              .ForMember(destinationMember => destinationMember.Email, options => options.MapFrom(src => src.Email))
              .ForMember(destinationMember => destinationMember.DateOfBirth, options => options.MapFrom(src => src.DateOfBirth))
              .ForMember(destinationMember => destinationMember.Gender, options => options.MapFrom(src => src.Gender))
              .ForMember(destinationMember => destinationMember.RoleId, options => options.MapFrom(src => src.RoleId)).ReverseMap();

            CreateMap<AttendanceDTO, Attendance>()
                .ForMember(a => a.Status, options => options.MapFrom(dto => dto.Status ? nameof(AttendanceStatusEnums.Present) : nameof(AttendanceStatusEnums.Absent)))
                .ForMember(a => a.UserId, options => options.MapFrom(dto => dto.UserId.Value))
                .ForMember(a => a.Id, options => options.MapFrom(dto => dto.AttendanceId))
                .ForMember(a => a.Date, options => options.MapFrom(dto => dto.Date))
                .ReverseMap();
            CreateMap<Pagination<Attendance>, Pagination<AttendanceViewDTO>>()
                .ReverseMap();
            CreateMap<Attendance, AttendanceViewDTO>()
                .ForMember(x => x.Id, options => options.MapFrom(dto => dto.Id))
                .ForMember(x => x.Date, options => options.MapFrom(dto => dto.Date))
                .ForMember(x => x.Status, options => options.MapFrom(dto => dto.Status))
                .ForMember(x => x.UserId, options => options.MapFrom(dto => dto.UserId))
                .ForMember(x => x.UserName, options => options.MapFrom(dto => dto.User.UserName))
                .ForMember(x => x.FullName, options => options.MapFrom(dto => dto.User.FullName))
                .ForMember(x => x.Email, options => options.MapFrom(dto => dto.User.Email))
                .ForMember(x => x.AvatarUrl, options => options.MapFrom(dto => dto.User.AvatarUrl))
                .ForMember(x => x.DateOfBirth, options => options.MapFrom(dto => dto.User.DateOfBirth))
                .ForMember(x => x.ApplicationId, options => options.MapFrom(dto => dto.ApplicationId))
                .ForMember(x => x.ApplicationReason, options => options.MapFrom(dto => dto.Application.Reason))
                .ForMember(x => x.TrainingClassId, options => options.MapFrom(dto => dto.TrainingClass.Id))
                .ForMember(x => x.ClassName, options => options.MapFrom(dto => dto.TrainingClass.Name))
                .ReverseMap();

            CreateMap<ResetPasswordDTO, User>()
                .ForMember(rp => rp.PasswordHash, opt => opt.MapFrom(src => src.NewPassword)).ReverseMap();
            CreateMap<User, UserViewModel>()
                .ForMember(desc => desc._Id, src => src.MapFrom(u => u.Id))
                .ReverseMap();

            CreateMap<SyllabusGeneralDTO, Syllabus>()
           .ForMember(ss => ss.SyllabusName, ss => ss.MapFrom(src => src.SyllabusName))
             .ForMember(ss => ss.CourseObjective, ss => ss.MapFrom(src => src.CourseObject))
               .ForMember(ss => ss.TechRequirements, ss => ss.MapFrom(src => src.TechRequirements))
               .ForMember(ss => ss.Duration, ss => ss.MapFrom(src => src.Duration))
               .ForMember(ss => ss.Level, ss => ss.MapFrom(src => src.Level))
               .ForMember(ss => ss.Status, ss => ss.MapFrom(src => "Active"))
               .ReverseMap();

            CreateMap<FinalViewSyllabusDTO, Syllabus>()
                .ForMember(x => x.SyllabusName, sf => sf.MapFrom(src => src.ProgramName))
                .ForMember(x => x.Status, sf => sf.MapFrom(src => src.Status))
                .ForMember(x => x.Level, sf => sf.MapFrom(src => src.General.Level))
                .ForMember(x => x.TechRequirements, sf => sf.MapFrom(src => src.General.TechnicalRequirements))
                .ForMember(x => x.CourseObjective, sf => sf.MapFrom(src => src.General.CourseObjectives))
                .ForMember(x => x.CreationDate, sf => sf.MapFrom(src => src.CreateOn))
                .ForMember(x => x.Id, sf => sf.MapFrom(src => src.ID))
                .ForMember(x => x.CreatedBy, sf => sf.MapFrom(src => src.CreateBy))
                .AfterMap((src, dest) =>
                {
                    src.Durations = new DurationView { TotalHours = dest.Duration };
                })
                .ReverseMap();
            //CreateMap<Syllabus, GeneralInformationDTO>()
            //    .ForMember(x => x.Id, s => s.MapFrom(src => src.Id))
            //    .ForMember(x => x.CreatedOn, s => s.MapFrom(src => src.CreationDate))
            //    .ForMember(x => x.ProgramName, s => s.MapFrom(src => src.SyllabusName))
            //    .ForMember(x => x.CreatedBy, s => s.MapFrom(src => src.CreatedBy))
            //   .ForPath(x => x.Duration.TotalHours, g => g.MapFrom(x => x.Duration))
            //    .ReverseMap();
            //CreateMap<Syllabus, ShowDetailSyllabusNewDTO>()
            //    .ForMember(x => x.Status, src => src.MapFrom(x => x.Status))
            //    .ForMember(x => x.Level, src => src.MapFrom(x => x.Level))
            //    .ForMember(x => x.CourseObjectives, src => src.MapFrom(x => x.CourseObjective))
            //    .ForMember(x => x.TechnicalRequirements, src => src.MapFrom(x => x.TechRequirements))
            //    .ReverseMap();
            CreateMap<Syllabus, SyllabusViewAllDTO>()
                .ForMember(x => x.ID, src => src.MapFrom(x => x.Id))
                .ForMember(x => x.SyllabusName, src => src.MapFrom(x => x.SyllabusName))
                //.ForMember(x=>x.syllabusStatus,src=>src.MapFrom(x=>x.Status))
                .ForMember(x => x.Duration, src => src.MapFrom(x => x.Duration))
                .ReverseMap();
            CreateMap<SyllabusViewForTrainingClassDetail, Syllabus>()
                .ForMember(x => x.Id, src => src.MapFrom(s => s.syllabus_id))
                .ForMember(x => x.SyllabusName, src => src.MapFrom(s => s.syllabus_name))
                .ForMember(x => x.Status, src => src.MapFrom(s => s.syllabus_status))
                .ForMember(x => x.Duration, src => src.MapFrom(s => s.syllabus_duration.TotalHours))
                .ReverseMap();

            CreateMap<UnitDTO, Unit>()
                    .ForMember(uu => uu.UnitName, uu => uu.MapFrom(src => src.UnitName))
                    .ForMember(uu => uu.TotalTime, uu => uu.MapFrom(src => src.TotalTime))
                    .ForMember(uu => uu.Session, uu => uu.MapFrom(src => src.Session))
                    .ReverseMap();
            //map lecture

            CreateMap<LectureDTO, Lecture>()
                    .ForMember(ll => ll.LectureName, ll => ll.MapFrom(src => src.LectureName))
                    .ForMember(ll => ll.OutputStandards, ll => ll.MapFrom(src => src.OutputStandards))
                    .ForMember(ll => ll.Duration, ll => ll.MapFrom(src => src.Duration))
                    .ForMember(ll => ll.DeliveryType, ll => ll.MapFrom(src => src.DeliveryType))
                    .ForMember(ll => ll.Status, ll => ll.MapFrom(src => src.Status))
                    .ReverseMap();


            CreateMap<User, LoginWithEmailDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            //map training class
            CreateMap<ExtendTrainingClassDTO2, ExtendTrainingClassDTO>()
                .ForMember(x => x.Attendees, src => src.MapFrom(x => x.Attendees))
                .ForMember(x => x.TimeFrame, src => src.MapFrom(x => x.TimeFrame))
                .IncludeMembers(x => x.General)
                .ReverseMap();
            CreateMap<GeneralDTO, ExtendTrainingClassDTO>()
                .ForMember(x => x.Admins, src => src.MapFrom(x => x.Admins))
                .ForMember(x => x.Trainers, src => src.MapFrom(x => x.Trainers))
                .ForMember(x => x.Fsu, src => src.MapFrom(x => x.Fsu))
                .IncludeMembers(x => x.Review, x => x.Approve)
                .ReverseMap();
            CreateMap<ReviewDTO, ExtendTrainingClassDTO>()
                .ForMember(x => x.ReviewDate, src => src.MapFrom(x => x.Date))
                .ForMember(x => x.ReviewAuthor, src => src.MapFrom(x => x.Author))
                .ReverseMap();
            CreateMap<ApproveDTO, ExtendTrainingClassDTO>()
                .ForMember(x => x.ApproveDate, src => src.MapFrom(x => x.Date))
                .ForMember(x => x.ApproveAuthor, src => src.MapFrom(x => x.Author))
                .ReverseMap();
            //Create class
            CreateMap<CreateTrainingClassDTO, TrainingClass>()
                .ForMember(x => x.TrainingClassAdmins, src => src.MapFrom(x => x.Admins))
                .ForMember(x => x.TrainingClassTrainers, src => src.MapFrom(x => x.Trainers))
                .ForMember(x => x.TrainingClassAttendee, src => src.MapFrom(x => x.Attendees))
                .ForMember(x => x.TrainingClassTimeFrame, src => src.MapFrom(x => x.TimeFrame))
                .ForPath(x => x.Location!.LocationName, src => src.MapFrom(x => x.LocationName))
                .ReverseMap();
            CreateMap<CreateTrainingClassDTO, Location>()
                .ForMember(x => x.LocationName, src => src.MapFrom(x => x.LocationName)).ReverseMap();
            CreateMap<CreateTrainingClassDTO2, CreateTrainingClassDTO>()
                .ForMember(x => x.Name, src => src.MapFrom(x => x.ClassName))
                .ForMember(x => x.Code, src => src.MapFrom(x => x.ClassCode))
                .ForMember(x => x.StatusClassDetail, src => src.MapFrom(x => x.ClassStatus))
                .IncludeMembers(x => x.General, x => x.TrainingPrograms)
                .IncludeBase<ExtendTrainingClassDTO2, ExtendTrainingClassDTO>()
                .ReverseMap();
            CreateMap<TrainingProgramDTO, CreateTrainingClassDTO>()
                .ForMember(x => x.TrainingProgramId, src => src.MapFrom(x => x.TrainingProgramId))
                .ReverseMap();
            CreateMap<ClassTimeDTO, CreateTrainingClassDTO>()
                .ForMember(x => x.StartTime, src => src.MapFrom(x => x.StartTime))
                .ForMember(x => x.EndTime, src => src.MapFrom(x => x.EndTime))
                .ReverseMap();
            CreateMap<GeneralDTO, CreateTrainingClassDTO>()
                .IncludeMembers(x => x.ClassTime)
                .ForMember(x => x.LocationName, src => src.MapFrom(x => x.Location))
                .ReverseMap();
            //Update class
            CreateMap<UpdateTrainingClassDTO, TrainingClass>()
                .ForMember(x => x.TrainingClassAdmins, src => src.MapFrom(x => x.Admins))
                .ForMember(x => x.TrainingClassTrainers, src => src.MapFrom(x => x.Trainers))
                .ForMember(x => x.TrainingClassAttendee, src => src.MapFrom(x => x.Attendees))
                .ForMember(x => x.TrainingClassTimeFrame, src => src.MapFrom(x => x.TimeFrame))
                .ForPath(x => x.Location!.LocationName, src => src.MapFrom(x => x.LocationName))
                .ReverseMap();
            CreateMap<UpdateTrainingClassDTO, Location>()
                .ForMember(x => x.LocationName, src => src.MapFrom(x => x.LocationName)).ReverseMap();
            CreateMap<UpdateTrainingClassDTO2, UpdateTrainingClassDTO>()
                .ForMember(x => x.Name, src => src.MapFrom(x => x.ClassName))
                .ForMember(x => x.Code, src => src.MapFrom(x => x.ClassCode))
                .ForMember(x => x.StatusClassDetail, src => src.MapFrom(x => x.ClassStatus))
                .IncludeMembers(x => x.General, x => x.TrainingPrograms)
                .IncludeBase<ExtendTrainingClassDTO2, ExtendTrainingClassDTO>()
                .ReverseMap();
            CreateMap<TrainingProgramDTO, UpdateTrainingClassDTO>()
                .ForMember(x => x.TrainingProgramId, src => src.MapFrom(x => x.TrainingProgramId))
                .ReverseMap();
            CreateMap<ClassTimeDTO, UpdateTrainingClassDTO>()
                .ForMember(x => x.StartTime, src => src.MapFrom(x => x.StartTime))
                .ForMember(x => x.EndTime, src => src.MapFrom(x => x.EndTime))
                .ReverseMap();
            CreateMap<GeneralDTO, UpdateTrainingClassDTO>()
                .IncludeMembers(x => x.ClassTime)
                .ForMember(x => x.LocationName, src => src.MapFrom(x => x.Location))
                .ReverseMap();

            CreateMap<TrainingClass, TrainingClassViewModel>()
                .ForMember(x => x._Id, src => src.MapFrom(x => x.Id))
                .IncludeMembers(x => x.Location)
                .ForMember(x => x.Admins, src => src.MapFrom(x => x.TrainingClassAdmins))
                .ForMember(x => x.Trainers, src => src.MapFrom(x => x.TrainingClassTrainers))
                .ForMember(x => x.Attendees, src => src.MapFrom(x => x.TrainingClassAttendee))
                .ForMember(x => x.TimeFrame, src => src.MapFrom(x => x.TrainingClassTimeFrame));
              
            CreateMap<Location, TrainingClassViewModel>()
                .ForMember(x => x.LocationName, src => src.MapFrom(x => x.LocationName));
            //view class
            CreateMap<AdminsDTO, TrainingClassAdmin>()
                .ForMember(x => x.AdminId, src => src.MapFrom(x => x.AdminId))
                .ReverseMap();
            CreateMap<TrainingClassFilterDTO, TrainingClass>()
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.ClassDuration.TotalHours))
                .ReverseMap();
            CreateMap<TrainerDTO, TrainingClassTrainer>()
                .ForMember(x => x.TrainerId, src => src.MapFrom(x => x.TrainerId))
                .ReverseMap();
            CreateMap<TimeFrameDTO, TrainingClassTimeFrame>()
                .ForMember(x => x.HighlightedDates, src => src.ConvertUsing(new HighLightDatesValueConverter()))
                //.AfterMap((s, d, context) => context.Mapper.Map(s.HighlightedDates, d.HighlightedDates))
                .ReverseMap()
                .ForMember(src => src.HighlightedDates, x => x.ConvertUsing(new CustomDateTimeValueConverter()));
            CreateMap<DateTime, HighlightedDates>()
                //.AfterMap((s, d, context) => context.Mapper.Map(s, d.HighlightedDate))
                .ConvertUsing<HighLightDatesTypeConverter>();
            CreateMap<HighlightedDates, DateTime>()
                //.AfterMap((s, d, context) => context.Mapper.Map(s.HighlightedDate, d))
                .ConvertUsing<CustomDateTimeTypeConverter>();
            CreateMap<AttendeesDTO, TrainingClassAttendees>()
                .ForMember(x => x.AttendeesActualNumber, src => src.MapFrom(x => x.AttendeesActualNumber))
                .ForMember(x => x.AttendeesPlannedNumber, src => src.MapFrom(x => x.AttendeesPlannedNumber))
                .ForMember(x => x.AttendeesAcceptedNumber, src => src.MapFrom(x => x.AttendeesAcceptedNumber))
                .ReverseMap();
            //CreateMap<TrainingClassDTO, TrainingClass>()
            //    .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.ClassDuration.TotalHours))
            //    .ReverseMap();
            CreateMap<TrainingClassViewDetail, TrainingClass>()
                .ForMember(x => x.Id, src => src.MapFrom(x => x.classId))
                .ForMember(x => x.Name, src => src.MapFrom(x => x.className))
                .ForMember(x => x.StatusClassDetail, src => src.MapFrom(x => x.classStatus))
                .ForMember(x => x.Code, src => src.MapFrom(x => x.classCode))
                .ForMember(x => x.Duration, src => src.MapFrom(x => x.classDuration.TotalHours))
                .ReverseMap();
            CreateMap<TrainingClassFilterDTO, TrainingClassViewAllDTO>()
                .ForMember(v => v.id, src => src.MapFrom(x => x.ClassID))
                .ForMember(v => v.className, src => src.MapFrom(x => x.Name))
                .ForMember(v => v.classCode, src => src.MapFrom(x => x.Code))
                .ForMember(v => v.fsu, src => src.MapFrom(x => x.Branch))
                .ForMember(v => v.location, src => src.MapFrom(x => x.LocationName))
                .ForMember(v => v.classDuration, src => src.MapFrom(x => x.ClassDuration))
                .ForMember(v => v.attendee, src => src.MapFrom(x => x.Attendee))
                .ForMember(v => v.createdBy, src => src.MapFrom(x => x.CreatedBy))
                .ForMember(v => v.createdOn, src => src.MapFrom(x => x.CreationDate))
                .ReverseMap();
            CreateMap<TrainingClassViewAllDTO, TrainingClass>()
                .ForMember(t => t.Id, src => src.MapFrom(x => x.id))
                .ForMember(t => t.Code, src => src.MapFrom(x => x.classCode))
                .ForMember(t=>t.Name,src=>src.MapFrom(x=>x.className))
                .ForMember(t=>t.CreationDate,src=>src.MapFrom(x=>x.createdOn))
                .ForMember(t=>t.Branch,src=>src.MapFrom(x=>x.fsu))
                .ForMember(t=>t.Attendee,src=>src.MapFrom(x=>x.attendee))
                .ForMember(t=>t.Duration,src=>src.MapFrom(x=>x.classDuration.TotalHours))
                .ReverseMap();
            CreateMap<TrainingClassFilterDTO, TrainingClass>()
              .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.ClassDuration.TotalHours))
              .ForMember(dest => dest.StartTime, opt => opt.MapFrom(x => x.StartDate))
              .ForMember(dest => dest.EndTime, opt => opt.MapFrom(x => x.EndDate))
              .ForMember(dest => dest.StatusClassDetail, opt => opt.MapFrom(x => x.Status))
              .ForMember(dest => dest.Branch, opt => opt.MapFrom(x => x.Branch))
              .ReverseMap();
            //map location
            CreateMap<CreateLocationDTO, Location>();
            CreateMap<Location, LocationDTO>()
                .ForMember(x => x._Id, src => src.MapFrom(x => x.Id));



            CreateMap<CreateTrainingProgramDTO, TrainingProgram>().ReverseMap();
            CreateMap<UpdateTrainingProgramDTO, TrainingProgram>().ReverseMap();

            CreateMap<Feedback, FeedbackModel>().ReverseMap();
            CreateMap<Feedback, FeedbackVM>().ReverseMap();

            CreateMap<TrainingProgramViewModel, TrainingProgram>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TrainingProgramId))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.TrainingTitle))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.TrainingStatus))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.TrainingDuration.TotalHours))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Modified.author))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.Modified.Date))
                .ReverseMap();

            CreateMap<SyllabusTrainingProgramViewModel, Syllabus>()
                .ForMember(dest => dest.SyllabusName, opt => opt.MapFrom(src => src.SyllabusName))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.Modified.author))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.Modified.Date))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.SyllabusDuration.TotalHours))
                .ReverseMap();


            CreateMap<CreateTrainingProgramDTO, TrainingProgram>().ReverseMap();
            CreateMap<UpdateTrainingProgramDTO, TrainingProgram>().ReverseMap();

            CreateMap<Applications, ApplicationDTO>()
                .ForMember(dest => dest.TrainingClassID, opt => opt.MapFrom(src => src.TrainingClassId))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.AbsentDateRequested, opt => opt.MapFrom(src => src.AbsentDateRequested)).ReverseMap();

            CreateMap<TrainingProgram, ViewAllTrainingProgramDTO>()
                .ForMember(destinationMember => destinationMember.Id, options => options.MapFrom(src => src.Id))
                .ForMember(destinationMember => destinationMember.TrainingTitle, options => options.MapFrom(src => src.ProgramName))
                .ForMember(destinationMember => destinationMember.CreationDate, options => options.MapFrom(src => src.CreationDate))
                .ForMember(destinationMember => destinationMember.CreatedBy, options => options.MapFrom(src => src.CreatedBy))
                .ForMember(destinationMember => destinationMember.Duration, options => options.MapFrom(src => src.Duration))
                .ForMember(destinationMember => destinationMember.Status, options => options.MapFrom(src => src.Status))
                .ReverseMap();

            CreateMap<AuditPlan, CreateAuditDTO>().ReverseMap();
            CreateMap<AuditQuestion, CreateAuditQuestionDTO>().ReverseMap();
            CreateMap<AuditPlanViewModel, AuditPlan>()
                .ReverseMap();
            CreateMap<AuditQuestion, AuditQuestionViewModel>().ReverseMap();

            CreateMap<Grading, GradingModel>().ReverseMap();
            CreateMap<AttendanceViewDTO, AttendanceMailDto>().ReverseMap();
            CreateMap<Attendance, AttendanceMailDto>()
                .ForMember(x => x.ClassName, options => options.MapFrom(dto => dto.TrainingClass.Name))
                .ForMember(x => x.Email, options => options.MapFrom(dto => dto.User.Email))
                .ForMember(x => x.FullName, options => options.MapFrom(dto => dto.User.FullName))
                .ReverseMap();

            CreateMap<CreateAuditSubmissionDTO, AuditSubmission>().ReverseMap();
            CreateMap<AuditSubmission, AuditSubmissionViewModel>().ReverseMap();
            CreateMap<AuditSubmission, UpdateSubmissionDTO>().ReverseMap();

            CreateMap<AssignmentViewModel, Assignment>()
                .ForMember(av => av.AssignmentName, a => a.MapFrom(src => src.AssignmentName))
                .ForMember(av => av.Description, a => a.MapFrom(src => src.Discription))
                .ForMember(av => av.LectureID, a => a.MapFrom(src => src.LectureID))
                .ForMember(av => av.DeadLine, a => a.MapFrom(src => src.DeadLine))
                .ReverseMap();

            CreateMap<CreateQuizIntoBankDTO, Question>()
                    .ForMember(cq => cq.TopicID, a => a.MapFrom(src => src.TopicID))
                    .ForMember(cq => cq.QuizTypeID, a => a.MapFrom(src => src.TypeID))
                    .ForMember(cq => cq.Content, a => a.MapFrom(src => src.Content))
                    .ForMember(cq => cq.Answer1, a => a.MapFrom(src => src.Answer1))
                    .ForMember(cq => cq.Answer2, a => a.MapFrom(src => src.Answer2))
                    .ForMember(cq => cq.Answer4, a => a.MapFrom(src => src.Answer3))
                    .ForMember(cq => cq.CorrectAnswer, a => a.MapFrom(src => src.CorrectAnswer))
                    .ReverseMap();

            CreateMap<UpdateQuestionDTO, Question>()
            .ForMember(cq => cq.TopicID, a => a.MapFrom(src => src.TopicID))
            .ForMember(cq => cq.QuizTypeID, a => a.MapFrom(src => src.TypeID))
            .ForMember(cq => cq.Content, a => a.MapFrom(src => src.Content))
            .ForMember(cq => cq.Answer1, a => a.MapFrom(src => src.Answer1))
            .ForMember(cq => cq.Answer2, a => a.MapFrom(src => src.Answer2))
            .ForMember(cq => cq.Answer4, a => a.MapFrom(src => src.Answer3))
            .ForMember(cq => cq.CorrectAnswer, a => a.MapFrom(src => src.CorrectAnswer))
            .ReverseMap();

            #region Mapping for hotfix UpdateSyllabus
            CreateMap<UpdateSyllabusModel, Syllabus>()
                .ForMember(x => x.Status, src => src.MapFrom(x => x.Status))
                .ForMember(x => x.SyllabusName, src => src.MapFrom(x => x.SyllabusName))
                .ForMember(x => x.Level, src => src.MapFrom(x => x.General.Level))
                .ForMember(x => x.CourseObjective, src => src.MapFrom(x => x.General.CourseObjective))
                .ForMember(x => x.TechRequirements, src => src.MapFrom(x => x.General.TechnicalRequirements))
                .ForMember(x => x.Code, src => src.MapFrom(x => x.Code))
                .ForMember(x => x.Duration, src => src.Ignore())
                .AfterMap((src, dest) =>
                {
                    src.General.CourseObjective = dest.CourseObjective;
                    src.General.Level = dest.Level;
                    src.General.TechnicalRequirements = dest.TechRequirements;
                })
                .ReverseMap();

            CreateMap<Unit, UpdateContentModel>()
                .ForMember(x => x.UnitName, src => src.MapFrom(x => x.UnitName))
                .ForMember(x => x.Hours, src => src.MapFrom(x => x.TotalTime))
                .ForMember(x => x.Unit, src => src.MapFrom(x => x.UnitNum))
                .ReverseMap();
            CreateMap<Lecture, UpdateLessonModel>()
                .ForMember(x => x.OutputStandards, src => src.MapFrom(x => x.OutputStandards))
                .ForMember(x => x.Status, src => src.MapFrom(x => x.Status))
                .ForMember(x => x.Name, src => src.MapFrom(x => x.LectureName))
                .ForMember(x => x.Hours, src => src.MapFrom(x => x.Duration))
                .ForMember(x => x.DeliveryType, src => src.MapFrom(x => x.DeliveryType))
                .ReverseMap();
            #endregion
            #region Mapping for SearchAndFilter User and TrainingProgram
            CreateMap<SearchAndFilterUserViewModel, User>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Fullname))
                .ReverseMap();
            CreateMap<SearchAndFilterTrainingProgramViewModel, TrainingProgram>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.TrainingTitle))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Durations.TotalHours))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ReverseMap();
            #endregion
        }
    }
}
