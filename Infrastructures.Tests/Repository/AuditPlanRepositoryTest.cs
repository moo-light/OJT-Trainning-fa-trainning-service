using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class AuditPlanRepositoryTest : SetupTest
    {
        private readonly AuditPlanRepository _auditPlanRepository;
        public AuditPlanRepositoryTest() 
        {
            _auditPlanRepository = new AuditPlanRepository(_dbContext, _currentTimeMock.Object, _claimsServiceMock.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnCorrectData() 
        {
            _auditPlanRepository.Should().BeAssignableTo<IAuditPlanRepository>();


            
               
               
        }
        
    }
}
