using Application.ViewModels.AuditModels;
using Application.ViewModels.AuditModels.UpdateModels;
using Application.ViewModels.AuditModels.ViewModels;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuditPlanService
    {
        public Task<AuditPlan?> CreateAuditPlan(CreateAuditDTO createAuditDTO);
        public Task<AuditPlanViewModel?> ViewDetailAuditPlan(Guid auditId);
        public Task<bool> UpdateAuditPlan(UpdateAuditDTO updateAuditDTO);
        public Task<bool> DeleteAuditPlan(Guid auditPlanId);
        Task<AuditPlan> GetAuditPlanById(Guid auditId);
    }
}
