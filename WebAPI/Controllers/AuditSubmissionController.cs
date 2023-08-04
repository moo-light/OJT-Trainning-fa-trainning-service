using Application.Interfaces;
using Application.Utils;
using Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.UpdateModels;
using Application.ViewModels.GradingModels;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditSubmissionController : ControllerBase
    {
        private readonly IAuditSubmissionService auditSubmissionService;
        private readonly IGradingService gradingService;
        private readonly IAuditPlanService auditPlanService;
        private readonly IDetailTrainingClassParticipateService _detailTrainingClassParticipateService;
        private readonly IClaimsService _claimsService;

       
        public AuditSubmissionController(IAuditSubmissionService auditSubmissionService, 
                                        IGradingService gradingService,
                                        IAuditPlanService auditPlanService,
                                        IDetailTrainingClassParticipateService detailTrainingClassParticipateService, 
                                        IClaimsService claimsService)                               
        {
            this.auditSubmissionService = auditSubmissionService;
            this.gradingService = gradingService;
            this.auditPlanService = auditPlanService;
            _claimsService = claimsService;
            _detailTrainingClassParticipateService = detailTrainingClassParticipateService;
        }
        [HttpPost]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Create))]
        public async Task<IActionResult> Create(CreateAuditSubmissionDTO createAuditSubmissionDTO)
        {

            var check = await _detailTrainingClassParticipateService.CheckJoinClass(createAuditSubmissionDTO.UserId, createAuditSubmissionDTO.ClassId);
            var userId = _claimsService.GetCurrentUserId;
            if((await _detailTrainingClassParticipateService.CheckJoinClass(userId, createAuditSubmissionDTO.ClassId)) is not null && check is not null) 
            {
                
            var result = await auditSubmissionService.CreateAuditSubmission(createAuditSubmissionDTO);
            
            if (result is not null)
            {
                    var auditPlan = await auditPlanService.GetAuditPlanById(result.AuditPlanId);
                    if (auditPlan is not null)
                    {
                        string letterGrade = "";
                        if (result.TotalGrade < 10 && result.TotalGrade >= 8) letterGrade = "A";
                        else if (result.TotalGrade < 8 && result.TotalGrade >= 6) letterGrade = "B";
                        else if (result.TotalGrade < 6 && result.TotalGrade >= 4) letterGrade = "C";
                        else if (result.TotalGrade < 4 && result.TotalGrade <= 2) letterGrade = "D";
                        else letterGrade = "F";
                        var gradingModel = new GradingModel(auditPlan.LectureId, check.Value,letterGrade,(int)result.TotalGrade);
                        await gradingService.CreateGradingAsync(gradingModel);
                        return Ok(result);

                    }
                    else return BadRequest("Can not found AuditPlan");
               

            }
            else return BadRequest("Can not create Submission");

            } else return BadRequest("Mentor/Trainer Not join class can not add Review");
        }

        [Authorize(Roles = "Admin,SuperAdmin,Trainer,Mentor")]
        [HttpGet("detail/{auditSubmissionId}")]
        public async Task<IActionResult> GetDetail(Guid auditSubmissionId)
        {
            var result = await auditSubmissionService.GetAuditSubmissionDetail(auditSubmissionId);
            if (result is not null) return Ok(result);
            else return BadRequest();

        }

        [HttpDelete]
        [Authorize(Roles = "Trainer,Mentor")]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> Delete(Guid auditSubmissionId)
        {
            var result = await auditSubmissionService.DeleteSubmissionDetail(auditSubmissionId);
            if (result) return NoContent();
            else return BadRequest();
        }

        [HttpPut]
        [Authorize]
        [ClaimRequirement(nameof(PermissionItem.ClassPermission), nameof(PermissionEnum.Modifed))]
        public async Task<IActionResult> Update(UpdateSubmissionDTO updateSubmissionDTO)
        {
            var check = await _detailTrainingClassParticipateService.CheckJoinClass(updateSubmissionDTO.UserId, updateSubmissionDTO.ClassId);
            var userId = _claimsService.GetCurrentUserId;
            if ((await _detailTrainingClassParticipateService.CheckJoinClass(userId, updateSubmissionDTO.ClassId)) is not null)
            {
                var result = await auditSubmissionService.UpdateSubmissionDetail(updateSubmissionDTO);
                if (result) return NoContent();
                else return BadRequest();
            }
            else return BadRequest("Can not Update AuditSubmission _ Not permited");
            
        }
        [Authorize]
        [HttpGet("{auditPlanId}")]
        public async Task<IActionResult> GetAllByAuditPlan(Guid auditPlanId)
        {
            var result = await auditSubmissionService.GetAllAuditSubmissionByAuditPlan(auditPlanId);
            if (result is not null) return Ok(result);
            else return BadRequest();
        }
    }
}
