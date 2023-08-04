using Application.Interfaces;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class TrainingProgramRepository : GenericRepository<TrainingProgram>, ITrainingProgramRepository
    {
        private readonly AppDbContext _appDbContext;
        public TrainingProgramRepository(AppDbContext dbContext,
       ICurrentTime timeService,
       IClaimsService claimsService)
       : base(dbContext,
        timeService,
             claimsService)
        {
            _appDbContext = dbContext;
        }

        public async Task<TrainingProgram> GetTraningProgramDetail(Guid trainingProgramId)
        => await _appDbContext.TrainingPrograms.Include(s => s.DetailTrainingProgramSyllabus).ThenInclude(s => s.Syllabus).FirstOrDefaultAsync();

    }
}
