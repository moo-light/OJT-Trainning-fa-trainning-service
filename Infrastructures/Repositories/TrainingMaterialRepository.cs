using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.TrainingClassModels;
using Application.ViewModels.TrainingMaterialModels;
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
    public class TrainingMaterialRepository : GenericRepository<TrainingMaterial>, ITrainingMaterialRepository
    {
        private readonly AppDbContext _dbContext;

        public TrainingMaterialRepository(AppDbContext context, ICurrentTime timeService, IClaimsService claimsService) : base(context, timeService, claimsService)
        {
            _dbContext = context;
        }

        public async Task DeleteTrainingMaterial(string blobName)
        {
            TrainingMaterial resultFile = await _dbContext.TrainingMaterials.Where(x => x.BlobName == blobName).FirstOrDefaultAsync();
            if (resultFile != null)
            {
                _dbContext.TrainingMaterials.Remove(resultFile);
              
            }
            
        }

        public async Task<List<TrainingMaterial>> GetAllFileWithLectureId(Guid lectureId)
        {
            var file = _dbContext.TrainingMaterials.Where(x => x.lectureID == (lectureId) && x.IsDeleted == false).ToList();

            if (file == null)
            {
                throw new Exception("No file was found");
            }
            return file;
        }

        public async Task<TrainingMaterialDTO> GetTrainingMaterial(Guid lectureId)
        {
            var getTrainingMaterial = _dbContext.TrainingMaterials
                                        .Where(x => x.IsDeleted == false && x.lectureID == (lectureId))
                                     .Select(t => new TrainingMaterialDTO
                                     {
                                         blobName = t.BlobName,
                                         createdBy = string.Join(",", _dbContext.Users.Where(x => x.Id == t.CreatedBy).Select(u => u.UserName)),
                                         createdOn = t.CreationDate,
                                     }).FirstOrDefault();
            return getTrainingMaterial;
        }

        public async Task<List<string>> GetAllDeletedTrainingMaterialNames()
        {
            var getList = _dbContext.TrainingMaterials
                .Where(x => x.IsDeleted == true)
                .Select(t => t.BlobName)
                .ToList();
            return getList;
        }
    }
}
