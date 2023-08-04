using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.TrainingClassModels;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{

    public class DetailTrainingClassParticipateRepository : GenericRepository<DetailTrainingClassParticipate>, IDetailTrainingClassParticipateRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        public DetailTrainingClassParticipateRepository(AppDbContext context, IMapper mapper, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _dbContext = context;
            _mapper = mapper;
        }


        public async Task<DetailTrainingClassParticipate> GetDetailTrainingClassParticipateAsync(Guid userId, Guid classId)
        {
            var result = _dbContext.DetailTrainingClassParticipates.SingleOrDefault(x => x.UserId == userId && x.TrainingClassID == classId);
            if (result == null)
            {
                throw new Exception("Detail does not exist!");
            }
            return result;
        }

        public Guid GetDetailTrainingClassParticipateByClassID(Guid classID)
        {
            Guid detailID = Guid.Empty;
            var result = _dbContext.DetailTrainingClassParticipates.Single(x => x.TrainingClassID == classID);
            if (result != null)
            {
                detailID = result.Id;
            }
            return detailID;
        }

        public async Task<List<ClassTrainerDTO>> GetDetailTrainingClassParticipatesByClassIDAsync(Guid classID)
        {
            var result = await _dbContext.DetailTrainingClassParticipates
                        .Include(de => de.User)
                        .Include(de => de.TrainingClass)
                        .Where(de => de.TrainingClassID == classID)
                        .Select(de => new ClassTrainerDTO
                        {
                            trainerId = de.User.Id,
                            name=de.User.UserName,
                            email=de.User.Email,
                            phone="0908761239"
                        }).ToListAsync();
            return result;
            
        }
        public async Task<List<ClassAdminDTO>> GetAdminInClasssByClassIDAsync(Guid id)
        {
            var result =await _dbContext.DetailTrainingClassParticipates
                        .Include(de => de.User)
                        .Include(de => de.TrainingClass)
                        .Where(de => de.TrainingClassID == id)
                        .Select(de => new ClassAdminDTO
                        {
                            adminID = de.CreatedBy,
                            name = _dbContext.Users.Where(u => u.Id == de.CreatedBy).Select(u => u.UserName).FirstOrDefault(),
                            email = _dbContext.Users.Where(u => u.Id == de.CreatedBy).Select(u => u.Email).FirstOrDefault(),
                            phone = "0907123497"
                        }).ToListAsync();
            return result;
        }

        public List<string> GetTraineeEmailsOfClass(Guid classId)
        {
            var detailTraineeClass = _dbContext.DetailTrainingClassParticipates.Include("User").Where(x => x.TrainingClassID == classId).ToList();
            List<string> emailList = null;
            foreach (var d in detailTraineeClass)
            {
                if (emailList == null)
                {
                    emailList = new List<string>();
                }
                emailList.Add(d.User.Email);
            }
            return emailList;
        }
    }
}
