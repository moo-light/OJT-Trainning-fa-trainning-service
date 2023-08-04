using Application.ViewModels.AuditModels;
using Application.ViewModels.AuditModels.UpdateModels;
using Application.ViewModels.AuditModels.ViewModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using FluentAssertions.Equivalency.Tracing;
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
    public class AuditPlanControllerTest : SetupTest
    {
        private readonly AuditPlanController auditPlanController;
        public AuditPlanControllerTest()
        {
            auditPlanController = new AuditPlanController(_auditPlanServiceMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnOk()
        {
            var createAuditDTO = _fixture.Build<CreateAuditDTO>().Without(x => x.CreateAuditQuestionDTOS).Create();
            var auditPlan = _mapperConfig.Map<AuditPlan>(createAuditDTO);
            _auditPlanServiceMock.Setup(x => x.CreateAuditPlan(It.IsAny<CreateAuditDTO>())).ReturnsAsync(auditPlan);
            var result = await auditPlanController.Create(createAuditDTO);

            result.Should().BeAssignableTo<OkObjectResult>();

        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest()
        {
            var createAuditDTO = _fixture.Build<CreateAuditDTO>().Without(x => x.CreateAuditQuestionDTOS).Create();
            var auditPlan = _mapperConfig.Map<AuditPlan>(createAuditDTO);
            _auditPlanServiceMock.Setup(x => x.CreateAuditPlan(It.IsAny<CreateAuditDTO>())).ReturnsAsync(auditPlan = null);
            var result = await auditPlanController.Create(createAuditDTO);

            result.Should().BeAssignableTo<BadRequestResult>();

        }


        [Fact]
        public async Task GetAuditDetail_ShouldReturnOkObjectResult()
        {
            var auditPlan = _fixture.Build<AuditPlan>().Without(x => x.Lecture)
                                                       .Without(x => x.AuditSubmissions)
                                                       .Without(x => x.DetailAuditQuestions)
                                                       .Create();
            var auditView = _mapperConfig.Map<AuditPlanViewModel>(auditPlan);
            _auditPlanServiceMock.Setup(x => x.ViewDetailAuditPlan(It.IsAny<Guid>())).ReturnsAsync(auditView);
            var result = await auditPlanController.GetDetail(auditPlan.Id);
            result.Should().BeAssignableTo<OkObjectResult>();

        }

        [Fact]
        public async Task GetAuditDetail_ShouldReturnBadRequest()
        {
            var auditPlan = _fixture.Build<AuditPlan>().Without(x => x.Lecture)
                                                       .Without(x => x.AuditSubmissions)
                                                       .Without(x => x.DetailAuditQuestions)
                                                       .Create();
            var auditView = _mapperConfig.Map<AuditPlanViewModel>(auditPlan);
            _auditPlanServiceMock.Setup(x => x.ViewDetailAuditPlan(It.IsAny<Guid>())).ReturnsAsync(auditView = null);
            var result = await auditPlanController.GetDetail(auditPlan.Id);
            result.Should().BeAssignableTo<BadRequestResult>();

        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            Guid auditPlan = Guid.NewGuid();
            _auditPlanServiceMock.Setup(x => x.DeleteAuditPlan(It.IsAny<Guid>())).ReturnsAsync(true);
            var result = await auditPlanController.Delete(auditPlan);
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest()
        {
            Guid auditPlan = Guid.NewGuid();
            _auditPlanServiceMock.Setup(x => x.DeleteAuditPlan(It.IsAny<Guid>())).ReturnsAsync(false);
            var result = await auditPlanController.Delete(auditPlan);
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent()
        {
            var updateDTO = _fixture.Build<UpdateAuditDTO>().Without(x => x.CreateAuditQuestionDTOS).Create();

            _auditPlanServiceMock.Setup(x => x.UpdateAuditPlan(It.IsAny<UpdateAuditDTO>())).ReturnsAsync(true);
            var result = await auditPlanController.Update(updateDTO);
            result.Should().BeAssignableTo<NoContentResult>();

        }

        [Fact]
        public async Task Update_ShouldReturnBadRequest()
        {
            var updateDTO = _fixture.Build<UpdateAuditDTO>().Without(x => x.CreateAuditQuestionDTOS).Create();

            _auditPlanServiceMock.Setup(x => x.UpdateAuditPlan(It.IsAny<UpdateAuditDTO>())).ReturnsAsync(false);
            var result = await auditPlanController.Update(updateDTO);
            result.Should().BeAssignableTo<BadRequestResult>();

        }

    }
}
