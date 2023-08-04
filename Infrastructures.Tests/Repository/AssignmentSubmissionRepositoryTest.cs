using Application.Repositories;
using Domains.Test;
using Infrastructures.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class AssignmentSubmissionRepositoryTest:SetupTest
    {
        private readonly IAssignmentSubmissionRepository _submissRepository;
        public AssignmentSubmissionRepositoryTest()
        {
            _submissRepository=new AssignmentSubmissionRepository(_dbContext,_currentTimeMock.Object,_claimsServiceMock.Object);
        }
        [Fact]
        public async Task TestAssignmentSubmissionRepository()
        {

        }
    }
}
