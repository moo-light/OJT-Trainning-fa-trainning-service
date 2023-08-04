using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.AuditModels;
using Application.ViewModels.AuditModels.UpdateModels;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditPlanController : ControllerBase
    {
        private readonly IAuditPlanService auditPlanService;
        public AuditPlanController(IAuditPlanService auditPlanService)
        {
            this.auditPlanService = auditPlanService;
        }

        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> Create(CreateAuditDTO createDTO)
        {
            var result = await auditPlanService.CreateAuditPlan(createDTO);
            if (result is not null) return Ok(result);
            else return BadRequest();
        }

        [HttpGet("{auditPlanId}")]
        public async Task<IActionResult> GetDetail(Guid auditPlanId)
        {
            var result = await auditPlanService.ViewDetailAuditPlan(auditPlanId);
            if (result != null) return Ok(result);
            else return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> Delete(Guid auditPlanId)
        {
            var result = await auditPlanService.DeleteAuditPlan(auditPlanId);
            if (result) return NoContent();
            else return BadRequest();
        }

        [HttpPut]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> Update(UpdateAuditDTO updateAuditDTO)
        {
            var result = await auditPlanService.UpdateAuditPlan(updateAuditDTO);
            if (result) return NoContent();
            else return BadRequest();
        }
    }
}
