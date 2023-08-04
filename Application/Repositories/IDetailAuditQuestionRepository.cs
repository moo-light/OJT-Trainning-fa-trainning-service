using Application.ViewModels.AuditModels.ViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IDetailAuditQuestionRepository : IGenericRepository<DetailAuditQuestion>
    {
        Task<IEnumerable<AuditQuestionViewModel>> GetAuditQuestionsByAuditId(Guid auditId);
    }
}
