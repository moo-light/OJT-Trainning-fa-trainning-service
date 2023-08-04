using Application.Services;
using Application.ViewModels.AuditModels;
using Application.ViewModels.AuditModels.UpdateModels;
using Application.ViewModels.AuditModels.ViewModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class AuditPlanServiceTest : SetupTest
    {
        private readonly AuditPlanService auditPlanService;
        public AuditPlanServiceTest()
        {
            auditPlanService = new AuditPlanService(_unitOfWorkMock.Object, _mapperConfig);
        }

        [Fact]
        public async Task CreateAuditPlan_ShouldReturnCorrectData()
        {
            // Arrange
            var createAuditDTO = _fixture.Build<CreateAuditDTO>().Create<CreateAuditDTO>();
            var auditPlan = _mapperConfig.Map<AuditPlan>(createAuditDTO);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.AddAsync(It.IsAny<AuditPlan>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.AuditQuestionRepository.AddAsync(It.IsAny<AuditQuestion>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.AddAsync(It.IsAny<DetailAuditQuestion>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditPlan);

            var actualResult = await auditPlanService.CreateAuditPlan(createAuditDTO);
            actualResult.Should().BeAssignableTo<AuditPlan>();
            actualResult.Should().NotBeNull();
            actualResult.AuditPlanName.Should().BeEquivalentTo(createAuditDTO.AuditPlanName);
        }

        [Fact]
        public async Task CreateAuditPlan_NotHaveAnyQuestion_ShouldReturnException()
        {
            var createAuditDTO = _fixture.Build<CreateAuditDTO>().Without(x => x.CreateAuditQuestionDTOS).Create<CreateAuditDTO>();
            var auditPlan = _mapperConfig.Map<AuditPlan>(createAuditDTO);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.AddAsync(It.IsAny<AuditPlan>())).Verifiable();

            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            Func<Task> act = async () => await auditPlanService.CreateAuditPlan(createAuditDTO);
            await act.Should().ThrowAsync<Exception>().WithMessage("Questions can not null");

        }

        [Fact]
        public async Task CreateAuditPlan_SaveChangeFailed()
        {
            var createAuditDTO = _fixture.Build<CreateAuditDTO>().Create<CreateAuditDTO>();

            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.AddAsync(It.IsAny<AuditPlan>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);
            Func<Task> act = async () => await auditPlanService.CreateAuditPlan(createAuditDTO);
            await act.Should().ThrowAsync<Exception>().WithMessage("Create Audit Plan Failed! Try Again!");
        }

        [Fact]
        public async Task GetDetailAuditPlan_ShouldReturnCorrectData()
        {
            var auditPlan = _fixture.Build<AuditPlan>().Without(x => x.Lecture)
                                                       .Without(x => x.AuditSubmissions)
                                                       .Without(x => x.DetailAuditQuestions)
                                                       .Create();
            var questions = _fixture.Build<AuditQuestionViewModel>().CreateMany(2);
            var auditView = _mapperConfig.Map<AuditPlanViewModel>(auditPlan);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditPlan);

            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.GetAuditQuestionsByAuditId(It.IsAny<Guid>())).ReturnsAsync(questions);

            var result = await auditPlanService.ViewDetailAuditPlan(auditPlan.Id);
            result.Should().BeAssignableTo<AuditPlanViewModel>();
            result.Should().NotBeNull();
            result!.AuditPlanName.Should().BeEquivalentTo(auditPlan.AuditPlanName);
        }

        [Fact]
        public async Task GetDetailAuditPlan_ShouldThrowException()
        {
            AuditPlan? auditPlan = null;
            Guid id = Guid.NewGuid();
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditPlan);
            Func<Task> act = async () => await auditPlanService.ViewDetailAuditPlan(id);
            await act.Should().ThrowAsync<Exception>().WithMessage("Not found! AuditPlan is not existed or has been deleted");
        }

        [Fact]
        public async Task DeleteAuditPlan_ShouldReturnTrue()
        {
            var auditPlan = _fixture.Build<AuditPlan>().Without(x => x.Lecture)
                                                      .Without(x => x.AuditSubmissions)
                                                      .Without(x => x.DetailAuditQuestions)
                                                      .Create();
            var detailAuditQuestion = _fixture.Build<DetailAuditQuestion>()
                                              .Without(x => x.AuditPlan)
                                              .Without(x => x.AuditQuestion)
                                              .Without(x => x.DetailAuditSubmissions)
                                              .CreateMany(2).ToList();
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditPlan);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.SoftRemove(It.IsAny<AuditPlan>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.FindAsync(x => x.AuditPlanId == auditPlan.Id)).ReturnsAsync(detailAuditQuestion);
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.SoftRemoveRange(It.IsAny<List<DetailAuditQuestion>>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var result = await auditPlanService.DeleteAuditPlan(auditPlan.Id);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAuditPlan_ShouldReturnFalse()
        {
            var auditPlan = _fixture.Build<AuditPlan>().Without(x => x.Lecture)
                                                      .Without(x => x.AuditSubmissions)
                                                      .Without(x => x.DetailAuditQuestions)
                                                      .Create();
            var Id = auditPlan.Id;
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditPlan = null);
            var result = await auditPlanService.DeleteAuditPlan(Id);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAuditPlan_ShouldReturnTrue()
        {
            var updateAuditDTO = _fixture.Build<UpdateAuditDTO>().Create();
            var audit = _mapperConfig.Map<AuditPlan>(updateAuditDTO);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(audit);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.Update(It.IsAny<AuditPlan>())).Verifiable();
            var detailAuditQuestion = _fixture.Build<DetailAuditQuestion>()
                                            .Without(x => x.AuditPlan)
                                            .Without(x => x.AuditQuestion)
                                            .Without(x => x.DetailAuditSubmissions)
                                            .CreateMany(2).ToList();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.FindAsync(x => x.AuditPlanId == updateAuditDTO.Id)).ReturnsAsync(detailAuditQuestion);
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.SoftRemoveRange(It.IsAny<List<DetailAuditQuestion>>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.AuditQuestionRepository.AddAsync(It.IsAny<AuditQuestion>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.AddAsync(It.IsAny<DetailAuditQuestion>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            var result = await auditPlanService.UpdateAuditPlan(updateAuditDTO);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAuditPlan_SaveChangesFail_ShouldThrowException()
        {
            var updateAuditDTO = _fixture.Build<UpdateAuditDTO>().Create();
            var audit = _mapperConfig.Map<AuditPlan>(updateAuditDTO);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(audit);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.Update(It.IsAny<AuditPlan>())).Verifiable();
            var detailAuditQuestion = _fixture.Build<DetailAuditQuestion>()
                                            .Without(x => x.AuditPlan)
                                            .Without(x => x.AuditQuestion)
                                            .Without(x => x.DetailAuditSubmissions)
                                            .CreateMany(2).ToList();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.FindAsync(x => x.AuditPlanId == updateAuditDTO.Id)).ReturnsAsync(detailAuditQuestion);
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.SoftRemoveRange(It.IsAny<List<DetailAuditQuestion>>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.AuditQuestionRepository.AddAsync(It.IsAny<AuditQuestion>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.AddAsync(It.IsAny<DetailAuditQuestion>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);

            Func<Task> act = async () => await auditPlanService.UpdateAuditPlan(updateAuditDTO);
            await act.Should().ThrowAsync<Exception>().WithMessage("Save Changes Fail");

        }

        [Fact]
        public async Task UpdateAuditPlan_NotFoundAnyAuditPlan_ShouldThrowException()
        {
            var updateAuditDTO = _fixture.Build<UpdateAuditDTO>().Without(x => x.CreateAuditQuestionDTOS).Create();
            var audit = _mapperConfig.Map<AuditPlan>(updateAuditDTO);
            _unitOfWorkMock.Setup(x => x.AuditPlanRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(audit = null);


            Func<Task> act = async () => await auditPlanService.UpdateAuditPlan(updateAuditDTO);
            await act.Should().ThrowAsync<Exception>().WithMessage("Cant find any Audit Plan");

        }

    }
}
