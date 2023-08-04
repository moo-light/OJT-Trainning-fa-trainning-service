using Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IDetailAuditSubmissionRepository : IGenericRepository<DetailAuditSubmission>
    {
        Task<IEnumerable<DetailAuditSubmissionViewModel>> GetDetailView(Guid auditSubmissionId);

    }
}
