using System.Linq.Expressions;
using Application.Services;
using Application.ViewModels.AuditModels.AuditSubmissionModels.CreateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.UpdateModels;
using Application.ViewModels.AuditModels.AuditSubmissionModels.ViewModels;
using Application.ViewModels.AuditModels.UpdateModels;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class AuditSubmissionServiceTest : SetupTest
    {
        private readonly AuditSubmissionService auditSubmissionSerivce;
        public AuditSubmissionServiceTest()
        {
            auditSubmissionSerivce = new AuditSubmissionService(_unitOfWorkMock.Object, _mapperConfig);
        }
        [Fact]
        public async Task CreateAuditSubmission_ShouldReturnCorrectData()
        {
            var auditSubmission = _fixture.Build<CreateAuditSubmissionDTO>().Create();
            var audit = _mapperConfig.Map<AuditSubmission>(auditSubmission);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.AddAsync(It.IsAny<AuditSubmission>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.AddRangeAsync(It.IsAny<List<DetailAuditSubmission>>())).Verifiable();
            var detailQuestion = _fixture.Build<DetailAuditQuestion>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Without(x => x.AuditQuestion).Create();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(detailQuestion);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(audit);
            var result = await auditSubmissionSerivce.CreateAuditSubmission(auditSubmission);
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<AuditSubmission>();
            result.Message.Should().BeEquivalentTo(auditSubmission.Message);
        }

        [Fact]
        public async Task CreateAuditSubmission_SaveChangesFailed_ShouldThrowException()
        {
            var auditSubmission = _fixture.Build<CreateAuditSubmissionDTO>().Create();
            var audit = _mapperConfig.Map<AuditSubmission>(auditSubmission);
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(0);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.AddAsync(It.IsAny<AuditSubmission>())).Verifiable();
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.AddRangeAsync(It.IsAny<List<DetailAuditSubmission>>())).Verifiable();

            Func<Task> act = async () => await auditSubmissionSerivce.CreateAuditSubmission(auditSubmission);
            await act.Should().ThrowAsync<Exception>().WithMessage("Save Changes Failed");

        }

        [Fact]
        public async Task CreateAuditSubmission_DetailNull_ShouldThrowException()
        {
            var auditSubmission = _fixture.Build<CreateAuditSubmissionDTO>().Without(x => x.AuditSubmissions).Create();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.AddAsync(It.IsAny<AuditSubmission>())).Verifiable();

            Func<Task> act = async () => await auditSubmissionSerivce.CreateAuditSubmission(auditSubmission);
            await act.Should().ThrowAsync<Exception>().WithMessage("Not have any Detail! Please try again");
        }

        [Fact]
        public async Task DeleteSubmission_ShouldReturnTrue()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(auditSubmission.Id)).ReturnsAsync(auditSubmission);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.SoftRemove(auditSubmission)).Verifiable();
            var submissionDetail = _fixture.Build<DetailAuditSubmission>().Without(x => x.AuditSubmission).Without(x => x.DetailAuditQuestion).Create();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.SoftRemoveRange(It.IsAny<List<DetailAuditSubmission>>())).Verifiable();

            var result = await auditSubmissionSerivce.DeleteSubmissionDetail(auditSubmission.Id);
            result.Should().BeTrue();
        }


        [Fact]
        public async Task DeleteSubmission_ShouldThrowException()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            var auditGuid = auditSubmission.Id;
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(auditSubmission.Id)).ReturnsAsync(auditSubmission = null);

            Func<Task> act = async () => await auditSubmissionSerivce.DeleteSubmissionDetail(auditGuid);
            await act.Should().ThrowAsync<Exception>().WithMessage("Not found any Submission");
        }

        [Fact]
        public async Task GetAuditSubmissionDetail_ShouldReturnCorrectData()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            var detailView = _fixture.Build<DetailAuditSubmissionViewModel>().CreateMany(2);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(auditSubmission.Id)).ReturnsAsync(auditSubmission);
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.GetDetailView(It.IsAny<Guid>())).ReturnsAsync(detailView);


            var result = await auditSubmissionSerivce.GetAuditSubmissionDetail(auditSubmission.Id);
            result.Should().NotBeNull();
            result.Message.Should().BeEquivalentTo(auditSubmission.Message);
        }
        [Fact]
        public async Task GetAuditSubmissionDetail_ShouldThrowException()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            var detailView = _fixture.Build<DetailAuditSubmissionViewModel>().CreateMany(2);
            var auditId = auditSubmission.Id;
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(auditSubmission.Id)).ReturnsAsync(auditSubmission);
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.GetDetailView(It.IsAny<Guid>())).ReturnsAsync(detailView = null);
            Func<Task> act = async () => await auditSubmissionSerivce.GetAuditSubmissionDetail(auditId);
            await act.Should().ThrowAsync<Exception>().WithMessage("Not have any detail submission");
        }
        [Fact]
        public async Task GetAuditSubmissionDetail_NotFound_ShouldThrowException()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            var auditId = auditSubmission.Id;
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditSubmission = null);
            Func<Task> act = async () => await auditSubmissionSerivce.GetAuditSubmissionDetail(auditId);
            await act.Should().ThrowAsync<Exception>().WithMessage("Can not find any Submission");
        }

        [Fact]
        public async Task UpdateAuditSubmission_ShouldReturnTrue()
        {
            var updateDTO = _fixture.Build<UpdateSubmissionDTO>().Create();
            var auditSubmission = _fixture.Build<AuditSubmission>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            Guid id = Guid.NewGuid();
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditSubmission);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.Update(auditSubmission)).Verifiable();
            var listDetailAuditSubmission = _fixture.Build<DetailAuditSubmission>().Without(x => x.DetailAuditQuestion).Without(x => x.AuditSubmission).Without(x => x.DetailAuditQuestion).CreateMany(1).ToList();
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.FindAsync(x => x.AuditSubmissionId == id && x.IsDeleted == false)).ReturnsAsync(listDetailAuditSubmission);

            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.SoftRemoveRange(listDetailAuditSubmission)).Verifiable();

            var listDetailSubmission = new List<DetailAuditSubmission>();
            var detailAuditQuestion = _fixture.Build<DetailAuditQuestion>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.GetByIdAsync(id)).ReturnsAsync(detailAuditQuestion);
            listDetailSubmission.Add(new DetailAuditSubmission());
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.AddRangeAsync(listDetailSubmission)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            var actualResult = await auditSubmissionSerivce.UpdateSubmissionDetail(updateDTO);
            actualResult.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAuditSubmission_NotFoundAuditSubmission_ShouldThrowException()
        {
            var updateDTO = _fixture.Build<UpdateSubmissionDTO>().Create();
            var auditSubmission = _fixture.Build<AuditSubmission>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            Guid id = Guid.NewGuid();
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auditSubmission = null);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.Update(It.IsAny<AuditSubmission>())).Verifiable();
            var listDetailAuditSubmission = _fixture.Build<DetailAuditSubmission>().Without(x => x.DetailAuditQuestion).Without(x => x.AuditSubmission).Without(x => x.DetailAuditQuestion).CreateMany(1).ToList();
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.FindAsync(x => x.AuditSubmissionId == id && x.IsDeleted == false)).ReturnsAsync(listDetailAuditSubmission);

            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.SoftRemoveRange(listDetailAuditSubmission)).Verifiable();

            var listDetailSubmission = new List<DetailAuditSubmission>();
            var detailAuditQuestion = _fixture.Build<DetailAuditQuestion>().Without(x => x.DetailAuditSubmissions).Without(x => x.AuditPlan).Create();
            _unitOfWorkMock.Setup(x => x.DetailAuditQuestionRepository.GetByIdAsync(id)).ReturnsAsync(detailAuditQuestion);
            listDetailSubmission.Add(new DetailAuditSubmission());
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.AddRangeAsync(listDetailSubmission)).Verifiable();
            _unitOfWorkMock.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);
            var result = await auditSubmissionSerivce.UpdateSubmissionDetail(updateDTO);
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetAuditSubmmissionByAuditPlan_ShouldReturnCorrectData()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>()
                                          .Without(x => x.AuditPlan)
                                          .Without(x => x.DetailAuditSubmissions)
                                          .CreateMany(1).ToList();
            var detailAuditSubmission = _fixture.Build<DetailAuditSubmissionViewModel>()
                                                    .CreateMany(2);
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.FindAsync(It.IsAny<Expression<Func<AuditSubmission, bool>>>())).ReturnsAsync(auditSubmission);
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.GetDetailView(It.IsAny<Guid>())).ReturnsAsync(detailAuditSubmission);
            var result = await auditSubmissionSerivce.GetAllAuditSubmissionByAuditPlan(auditSubmission.First().AuditPlanId);
            result.Count().Should().BeGreaterThanOrEqualTo(1);
        } 

        [Fact]
        public async Task GetAuditSubmmissionByAuditPlan_NotFoundSubmission_ShouldThrowException()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>()
                                          .Without(x => x.AuditPlan)
                                          .Without(x => x.DetailAuditSubmissions)
                                          .CreateMany(0).ToList();

            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.FindAsync(It.IsAny<Expression<Func<AuditSubmission, bool>>>()))!.ReturnsAsync(auditSubmission);
            
            Func<Task> act = async() => await auditSubmissionSerivce.GetAllAuditSubmissionByAuditPlan(It.IsAny<Guid>());
            await act.Should().ThrowAsync<Exception>().WithMessage("Not have any AuditSubmission");
        }

        [Fact]
        public async Task GetAuditSubmissionByAuditPlan_NotHaveAnyDetail_ShouldThrowException()
        {
            var auditSubmission = _fixture.Build<AuditSubmission>()
                                          .Without(x => x.AuditPlan)
                                          
                                          .Without(x => x.DetailAuditSubmissions)
                                          .CreateMany(1).ToList();
            var detailAuditSubmission = _fixture.Build<DetailAuditSubmission>()
                                                .Without(x => x.AuditSubmission)
                                                .Without(x => x.DetailAuditQuestion)
                                                .With(x => x.AuditSubmissionId, auditSubmission.First()!.Id)
                                                .CreateMany(2).ToList();
            _unitOfWorkMock.Setup(x => x.AuditSubmissionRepository.FindAsync(It.IsAny<Expression<Func<AuditSubmission, bool>>>())).ReturnsAsync(auditSubmission);
            _unitOfWorkMock.Setup(x => x.DetailAuditSubmissionRepository.FindAsync(It.IsAny<Expression<Func<DetailAuditSubmission, bool>>>()))!.ReturnsAsync(detailAuditSubmission = null);
            Func<Task> act = async () => await auditSubmissionSerivce.GetAllAuditSubmissionByAuditPlan(It.IsAny<Guid>());
            await act.Should().ThrowAsync<Exception>().WithMessage("Not have any detail submission");
        }
    }
}
