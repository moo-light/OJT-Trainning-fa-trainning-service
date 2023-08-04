using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels;
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
    public class LectureRepository : GenericRepository<Lecture>, ILectureRepository
    {
        private readonly AppDbContext _dbContext;

        public LectureRepository(AppDbContext dbContext,
         ICurrentTime timeService,
         IClaimsService claimsService)
         : base(dbContext,
               timeService,
               claimsService)
        {
            _dbContext = dbContext;
        }
        public Guid GetLectureIdByName(string name)
        {
            //query to get id through name
            var id = from a in _dbContext.Lectures
                     where a.LectureName.Equals(name)
                     select a.Id;

            //convert result to Guid
            Guid idResult = Guid.Parse(id.FirstOrDefault().ToString());
            return idResult;
        }
        public async Task<IEnumerable<Lecture>> GetLectureBySyllabusId(Guid syllabusId)
        {
            var result = from l in _dbContext.Lectures
                         join d in _dbContext.DetailUnitLecture on l.Id equals d.LectureID
                         join u in _dbContext.Units on d.UnitId equals u.Id
                         join s in _dbContext.Syllabuses on u.SyllabusID equals s.Id
                         where s.Id == syllabusId
                         select new Lecture
                         {
                             Id = l.Id,
                             DeliveryType = l.DeliveryType,
                             CreationDate = l.CreationDate,
                             CreatedBy = l.CreatedBy,
                             OutputStandards = l.OutputStandards,
                             LectureName = l.LectureName,
                             Duration = l.Duration,
                             Status = l.Status,
                             IsDeleted = l.IsDeleted,
                             ModificationDate = l.ModificationDate,
                             ModificationBy = l.ModificationBy

                         };

            return await result.ToListAsync();
        }

    }
}
