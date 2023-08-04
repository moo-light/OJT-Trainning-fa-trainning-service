using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingProgramModels.TrainingProgramView;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class DetailTrainingProgramSyllabusRepository : GenericRepository<DetailTrainingProgramSyllabus>, IDetailTrainingProgramSyllabusRepository
    {
        private readonly AppDbContext _dbContext;
        public DetailTrainingProgramSyllabusRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }


        //To take detailTrainingProgram
        public Guid TakeDetailTrainingID(Guid user_id, Guid training_class_id)
        {
            Guid guid = new Guid();
            var ketqua = from trainingclass in _dbContext.TrainingClasses
                         join detailtrainingclass in _dbContext.DetailTrainingClassParticipates on trainingclass.Id equals detailtrainingclass.TrainingClassID
                         where trainingclass.Id == training_class_id && detailtrainingclass.UserId == user_id
                         //&& detailtrainingclass.StatusClassDetail.Equals("Active")
                         select new { guid = detailtrainingclass.Id };

            return guid;
        }

     public  List<SyllabusViewForTrainingClassDetail> GetDetailByClassID(Guid programID)
        {
           var detailSyllabus =_dbContext.DetailTrainingProgramSyllabuses
                                   .Include(de=>de.Syllabus)
                                   .Include(de=>de.TrainingProgram)
                                   .Where(de=>de.TrainingProgram.Id==programID)
                                   .Select(de=>new SyllabusViewForTrainingClassDetail
                                   {
                                       syllabus_id=de.Syllabus.Id,
                                       trainerAvatarsUrl=_dbContext.Users.Where(u=>u.Id==de.Syllabus.CreatedBy).Select(u=>u.AvatarUrl).ToList(),
                                       syllabus_name=de.Syllabus.SyllabusName,
                                       syllabus_duration=new DurationView()
                                       {
                                           TotalHours=de.Syllabus.Duration,
                                       },
                                       syllabus_status=de.Syllabus.Status,
                                       lastEdit = new LastEditDTO()
                                       {
                                           modificationBy=_dbContext.Users.Where(u=>u.Id==de.Syllabus.ModificationBy).Select(u=>u.UserName).FirstOrDefault(),
                                           modificationDate=de.Syllabus.ModificationDate
                                       }
                                   }).ToList();
            return detailSyllabus;
        }
    }
}
