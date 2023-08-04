using Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.UpdateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels;
using Application.ViewModels.GradingModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace WebAPI.Tests.Controllers
{
    public class AuditSubmissionControllerTest : SetupTest
    {
        private readonly AuditSubmissionController auditSubmissionController;

        public AuditSubmissionControllerTest()
        {
            auditSubmissionController = new AuditSubmissionController(_auditSubmissionServiceMock.Object,
             _gradingServiceMock.Object,
              _auditPlanServiceMock.Object,
             _detailTrainingClassParticipateService.Object, 
             _claimsServiceMock.Object);        
             }
        
        [Fact]
        public async Task Create_ShouldReturn200()
        {
            var audit = _fixture.Build<AuditSubmission>()
                                .Without(x => x.DetailAuditSubmissions)
                                .Without(x => x.AuditPlan)
                                .Create();
            var auditPlan = _fixture.Build<AuditPlan>()
                                    .Without(x => x.AuditSubmissions)
                                    .Without(x => x.DetailAuditQuestions)
                                    .Without(x => x.Lecture)
                                    .Create();
            var detailId = Guid.NewGuid();
            _auditSubmissionServiceMock.Setup(x => x.CreateAuditSubmission(It.IsAny<CreateAuditSubmissionDTO>())).ReturnsAsync(audit);
            _detailTrainingClassParticipateService.Setup(x => x.CheckJoinClass(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(detailId);
            _auditPlanServiceMock.Setup(x => x.GetAuditPlanById(It.IsAny<Guid>())).ReturnsAsync(auditPlan);
            _gradingServiceMock.Setup(x => x.CreateGradingAsync(It.IsAny<GradingModel>())).Verifiable();
            var result = await auditSubmissionController.Create(_mapperConfig.Map<CreateAuditSubmissionDTO>(audit));
            result.Should().BeAssignableTo<OkObjectResult>();
        }

        [Fact]
        public async Task Create_NotJoinClass_ShouldReturn400()
        {
            var audit = _fixture.Build<AuditSubmission>()
                               .Without(x => x.DetailAuditSubmissions)
                               .Without(x => x.AuditPlan)
                               .Create();
            var auditPlan = _fixture.Build<AuditPlan>()
                                    .Without(x => x.AuditSubmissions)
                                    .Without(x => x.DetailAuditQuestions)
                                    .Without(x => x.Lecture)
                                    .Create();
            Guid? detailId = Guid.NewGuid();
            
            _detailTrainingClassParticipateService.Setup(x => x.CheckJoinClass(It.IsAny<Guid>(), It.IsAny<Guid>()))!.ReturnsAsync(detailId = null);
            
            var result = await auditSubmissionController.Create(_mapperConfig.Map<CreateAuditSubmissionDTO>(audit));
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            var actualResult = result as BadRequestObjectResult;
            actualResult!.Value.Should().BeEquivalentTo("Mentor/Trainer Not join class can not add Review");
        }

        [Fact]
        public async Task Create_NotHaveAuditPlan_ShouldReturn400()
        {
            var audit = _fixture.Build<AuditSubmission>()
                               .Without(x => x.DetailAuditSubmissions)
                               .Without(x => x.AuditPlan)
                               .Create();
            var auditPlan = _fixture.Build<AuditPlan>()
                                    .Without(x => x.AuditSubmissions)
                                    .Without(x => x.DetailAuditQuestions)
                                    .Without(x => x.Lecture)
                                    .Create();
            var detailId = Guid.NewGuid();
            _auditSubmissionServiceMock.Setup(x => x.CreateAuditSubmission(It.IsAny<CreateAuditSubmissionDTO>())).ReturnsAsync(audit);
            _detailTrainingClassParticipateService.Setup(x => x.CheckJoinClass(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(detailId);
            _auditPlanServiceMock.Setup(x => x.GetAuditPlanById(It.IsAny<Guid>()))!.ReturnsAsync(auditPlan = null);
            _gradingServiceMock.Setup(x => x.CreateGradingAsync(It.IsAny<GradingModel>())).Verifiable();
            var result = await auditSubmissionController.Create(_mapperConfig.Map<CreateAuditSubmissionDTO>(audit));
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            var actualResult = result as BadRequestObjectResult;
            actualResult!.Value.Should().BeEquivalentTo("Can not found AuditPlan");
        }
        [Fact]
        public async Task CreateAuditSubmission_CreateSubmissionFailed_ShouldReturn400()
        {
            var audit = _fixture.Build<AuditSubmission>()
                                .Without(x => x.DetailAuditSubmissions)
                                .Without(x => x.AuditPlan)
                                .Create();
            var resultAudit = _mapperConfig.Map<CreateAuditSubmissionDTO>(audit);
            var detailId = Guid.NewGuid();
            _detailTrainingClassParticipateService.Setup(x => x.CheckJoinClass(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(detailId);
            _auditSubmissionServiceMock.Setup(x => x.CreateAuditSubmission(It.IsAny<CreateAuditSubmissionDTO>()))!.ReturnsAsync(audit = null);
           
         
          
            var result = await auditSubmissionController.Create(resultAudit);
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            var actualResult = result as BadRequestObjectResult;
            actualResult!.Value.Should().BeEquivalentTo("Can not create Submission");
        }
        [Fact]
        public async Task GetDetail_ShouldReturn200()
        {
            var audit = _fixture.Build<AuditSubmissionViewModel>().Without(x => x.DetailAuditSubmisisonViewModel).Without(x => x.DetailAuditSubmisisonViewModel).Create();
            _auditSubmissionServiceMock.Setup(x => x.GetAuditSubmissionDetail(It.IsAny<Guid>())).ReturnsAsync(audit);
            var result = await auditSubmissionController.GetDetail(It.IsAny<Guid>());
            result.Should().BeAssignableTo<OkObjectResult>();
        }

        [Fact]
        public async Task GetDetail_ShouldReturn400()
        {
            var audit = _fixture.Build<AuditSubmissionViewModel>().Without(x => x.DetailAuditSubmisisonViewModel).Without(x => x.DetailAuditSubmisisonViewModel).Create();
            _auditSubmissionServiceMock.Setup(x => x.GetAuditSubmissionDetail(It.IsAny<Guid>()))!.ReturnsAsync(audit = null);
            var result = await auditSubmissionController.GetDetail(It.IsAny<Guid>());
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            _auditSubmissionServiceMock.Setup(x => x.DeleteSubmissionDetail(It.IsAny<Guid>())).ReturnsAsync(true);
            var result = await auditSubmissionController.Delete(It.IsAny<Guid>());
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest()
        {
            _auditSubmissionServiceMock.Setup(x => x.DeleteSubmissionDetail(It.IsAny<Guid>())).ReturnsAsync(false);
            var result = await auditSubmissionController.Delete(It.IsAny<Guid>());
            result.Should().BeAssignableTo<BadRequestResult>();
        }
        [Fact]
        public async Task Update_ShouldReturnBadRequest()
        {
            Guid? id = Guid.NewGuid();
            var audit = _fixture.Build<AuditSubmission>()
                             .Without(x => x.DetailAuditSubmissions)
                             .Without(x => x.AuditPlan)
                             .Create();
            
            _detailTrainingClassParticipateService.Setup(x => x.CheckJoinClass(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(id);
            _auditSubmissionServiceMock.Setup(x => x.UpdateSubmissionDetail(It.IsAny<UpdateSubmissionDTO>())).ReturnsAsync(false);
            var result = await auditSubmissionController.Update(_mapperConfig.Map<UpdateSubmissionDTO>(audit)); 
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent()
        {
            Guid? id = Guid.NewGuid();
            var audit = _fixture.Build<AuditSubmission>()
                             .Without(x => x.DetailAuditSubmissions)
                             .Without(x => x.AuditPlan)
                             .Create();

            _detailTrainingClassParticipateService.Setup(x => x.CheckJoinClass(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(id);
            _auditSubmissionServiceMock.Setup(x => x.UpdateSubmissionDetail(It.IsAny<UpdateSubmissionDTO>())).ReturnsAsync(true);
            var result = await auditSubmissionController.Update(_mapperConfig.Map<UpdateSubmissionDTO>(audit));
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Fact]
        public async Task Update_NotJoinClass_ShouldReturnBadRequest()
        {
            Guid? id = Guid.NewGuid();
            var audit = _fixture.Build<AuditSubmission>()
                             .Without(x => x.DetailAuditSubmissions)
                             .Without(x => x.AuditPlan)
                             .Create();
            var update = _mapperConfig.Map<UpdateSubmissionDTO>(audit);
            _detailTrainingClassParticipateService.Setup(x => x.CheckJoinClass(It.IsAny<Guid>(), It.IsAny<Guid>()))!.ReturnsAsync(id = null);
           
            var result = await auditSubmissionController.Update(update);
            result.Should().BeAssignableTo<BadRequestObjectResult>();
            var actualResult = result as BadRequestObjectResult;
            actualResult!.Value.Should().BeEquivalentTo("Can not Update AuditSubmission _ Not permited");
        }
    }
}
