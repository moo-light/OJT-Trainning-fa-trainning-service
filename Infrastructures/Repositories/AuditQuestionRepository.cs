using Application.Interfaces;
using Application.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class AuditQuestionRepository : GenericRepository<AuditQuestion>, IAuditQuestionRepository
    {
        private readonly AppDbContext _dbContext;

        public AuditQuestionRepository(AppDbContext dbContext,
        ICurrentTime timeService,
        IClaimsService claimsService)
        : base(dbContext,
              timeService,
              claimsService)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AuditQuestion>> GetAuditQuestionsByAuditPlanId(Guid auditPlanId)
        {
            // Old Query
            /*            var questions = from a in _dbContext.AuditQuestions
                                        join d in _dbContext.DetailAuditQuestions
                                        on a.Id equals d.AuditQuestionId
                                        where a.IsDeleted == false && d.IsDeleted == false && d.AuditPlanId == auditPlanId
                                        select new AuditQuestion
                                        {
                                            Id = a.Id,
                                            Description = a.Description
                                        };*/

            // Newer Query
            var questions = _dbContext.DetailAuditQuestions.Include(x => x.AuditQuestion)
                            .Where(x => x.IsDeleted == false && x.AuditPlanId == auditPlanId)
                            .Select(x => new AuditQuestion { Id = x.AuditQuestionId, Description = x.AuditQuestion.Description });
            if (questions is not null) return await questions.ToListAsync();
            else throw new Exception("Can not find any AuditQuestion");

        }
    }
}
