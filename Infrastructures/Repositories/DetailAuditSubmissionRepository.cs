using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Repositories
{
    public class DetailAuditSubmissionRepository : GenericRepository<DetailAuditSubmission>, IDetailAuditSubmissionRepository
    {
        private readonly AppDbContext _dbContext;
        public DetailAuditSubmissionRepository(AppDbContext dbContext,
        ICurrentTime timeService,
        IClaimsService claimsService)
        : base(dbContext,
              timeService,
              claimsService)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<DetailAuditSubmissionViewModel>> GetDetailView(Guid auditSubmissionId)
        {

            // Old Query
            /*            var result = from d in _dbContext.DetailAuditSubmissions
                                     join dq in _dbContext.DetailAuditQuestions
                                     on d.DetailAuditQuestionId equals dq.Id
                                     join q in _dbContext.AuditQuestions
                                     on dq.AuditQuestionId equals q.Id
                                     where d.AuditSubmissionId == auditSubmissionId
                                     select new DetailAuditSubmissionViewModel
                                     {
                                         Question = q.Description,
                                         Answer = d.Answer
                                     };*/
            var result = _dbContext.DetailAuditSubmissions
                                   .Include(x => x.DetailAuditQuestion).ThenInclude(d => d.AuditQuestion)
                                   .Include(x => x.AuditSubmission)
                                   .Where(auditSubmission => auditSubmission.AuditSubmissionId == auditSubmissionId)
                                   .Select(auditSubmission => new DetailAuditSubmissionViewModel
                                   {
                                       Question = auditSubmission.DetailAuditQuestion.AuditQuestion.Description,
                                       Answer = auditSubmission.Answer
                                   });
            return await result.ToListAsync();

        }
    }
}
