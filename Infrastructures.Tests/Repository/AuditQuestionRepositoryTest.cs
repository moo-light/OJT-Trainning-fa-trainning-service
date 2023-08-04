using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class AuditQuestionRepositoryTest : SetupTest
    {
        private readonly IAuditQuestionRepository auditQuestionRepository;
        private readonly IEnumerable<AuditQuestion> auditQuestions;
        private readonly List<DetailAuditQuestion> detailQuestions;
        private readonly AuditPlan auditPlan;
        public AuditQuestionRepositoryTest()
        {
            auditQuestions = _fixture.Build<AuditQuestion>()
                                .With(x => x.IsDeleted, false)
                                .CreateMany(1);
            auditPlan = _fixture.Build<AuditPlan>()
                                .Without(x => x.Lecture)
                                .Without(x => x.DetailAuditQuestions)
                                .With(x => x.IsDeleted, false)
                                .Create();
            detailQuestions = _fixture.Build<DetailAuditQuestion>()
                .Without(x => x.AuditPlan)
                .Without(x => x.DetailAuditSubmissions)
                .With(x => x.AuditPlanId,auditPlan.Id)
                .With(x => x.IsDeleted, false)
                .With(x => x.AuditQuestionId, auditQuestions.First().Id)
                .Without(x => x.AuditQuestion).CreateMany(1)
                
                .ToList();
            _dbContext.AuditQuestions.AddRange(auditQuestions);
            _dbContext.AuditPlans.Add(auditPlan);
            _dbContext.DetailAuditQuestions.AddRange(detailQuestions);
            _dbContext.SaveChanges();
            auditQuestionRepository = new AuditQuestionRepository(_dbContext, _currentTimeMock.Object, _claimsServiceMock.Object);     
        }

        [Fact]
        public async Task GetAuditQuestionsByAuditPlan_ShouldReturnCorrectData()
        {
            var result = await auditQuestionRepository.GetAuditQuestionsByAuditPlanId(auditPlan.Id);
            result.Should().BeAssignableTo<IEnumerable<AuditQuestion>>();
            result.Count().Should().BeGreaterThanOrEqualTo(1);
        }
    }
}
