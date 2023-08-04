using Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.UpdateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuditSubmissionService
    {
        Task<AuditSubmission> CreateAuditSubmission(CreateAuditSubmissionDTO createAuditSubmissionDTO);

        Task<AuditSubmissionViewModel> GetAuditSubmissionDetail(Guid auditSubmissionId);
        Task<bool> UpdateSubmissionDetail(UpdateSubmissionDTO updateSubmisionDTO);
        Task<bool> DeleteSubmissionDetail(Guid auditSubmissionId);
        Task<IEnumerable<AuditSubmissionViewModel>> GetAllAuditSubmissionByAuditPlan(Guid auditPlanId);


    }
}
